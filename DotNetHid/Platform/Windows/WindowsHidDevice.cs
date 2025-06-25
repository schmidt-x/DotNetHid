using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DotNetHid.Enums;
using DotNetHid.Platform.Shared;
using DotNetHid.Platform.Windows.Interop;
using DotNetHid.Platform.Windows.SafeHandles;
using static DotNetHid.Platform.Windows.Interop.Constants;

namespace DotNetHid.Platform.Windows;

internal class WindowsHidDevice : HidDevice
{
	private readonly bool _outputWarnings; // TODO: do I really need it?
	
	private HANDLE? _handle;
	private bool _isOpen;
	
	// Used throughout the lifetime of this device instance.
	// Since the device remains reusable after calling .Dispose() (which just calls .Close()),
	// this handle is not disposed manually and will be released only when its finalizer is called by the GC. 
	private readonly HANDLE _manualEvent;

	internal WindowsHidDevice(HidDeviceInfo info, bool outputWarnings) : base(info)
	{
		var hEvent = Kernel32.CreateEventW(IntPtr.Zero, true, false, null);
		if (hEvent.IsInvalid)
		{
			int errorCode = Marshal.GetLastPInvokeError();
			throw new SystemException($"Failed to create event: {Helpers.GetFormattedErrorMessage(errorCode)}");
		}
		
		_manualEvent = hEvent;
		_outputWarnings = outputWarnings;
	}
	
	public override HidError? Open(DeviceAccess deviceAccess)
	{
		if (_isOpen)
		{
			Debug.Assert(_handle is { IsInvalid: false, IsClosed: false });
			return null;
		}
		
		Debug.Assert(_handle is null);
		
		if (deviceAccess is < DeviceAccess.Read or > DeviceAccess.ReadWrite)
		{
			return new HidError(ErrorKind.InvalidArgument, "Invalid device access mode.");
		}
		
		UInt32 desiredAccess = ((deviceAccess & DeviceAccess.Read) == DeviceAccess.Read ? GENERIC_READ : 0) |
		                       ((deviceAccess & DeviceAccess.Write) == DeviceAccess.Write ? GENERIC_WRITE : 0);
		
		const UInt32 shareMode = FILE_SHARE_READ | FILE_SHARE_WRITE;
		
		var handle = Kernel32.CreateFileW(
			Info.DevicePath, desiredAccess, shareMode, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);
		
		if (handle.IsInvalid == false)
		{
			_handle = handle;
			_isOpen = true;
			return null;
		}
		
		int errorCode = Marshal.GetLastPInvokeError();
		return errorCode switch
		{
			ERROR_FILE_NOT_FOUND    => Errors.DeviceNotConnected,
			ERROR_SHARING_VIOLATION => Errors.DeviceInUse,
			_                       => new HidError(ErrorKind.Other, Helpers.GetFormattedErrorMessage(errorCode))
		};
	}

	// TODO: consider moving to the base class
	public override void Close()
	{
		if (!_isOpen)
		{
			Debug.Assert(_handle is null);
			return;
		}
		
		Debug.Assert(_handle is { IsInvalid: false, IsClosed: false });
		_handle.Close();
		
		_handle = null;
		_isOpen = false;
	}
	
	protected override HidError? InternalWrite(ReadOnlySpan<byte> buffer)
	{
		Debug.Assert(buffer.Length == Info.OutputReportByteLength);
		
		bool mustClose = false;
		
		if (!_isOpen) // If the device wasn't open initially, we'll open it and close it after we're done.
		{
			if (!TryOpen(DeviceAccess.Write, out var error)) return error;
			mustClose = true;
		}
		
		Debug.Assert(_handle is { IsInvalid: false, IsClosed: false });
		
		try
		{
			var ol = new Structs.OVERLAPPED(_manualEvent.DangerousGetHandle());
			
			if (Kernel32.WriteFile(_handle, ref MemoryMarshal.GetReference(buffer), Info.OutputReportByteLength, out _, ref ol))
			{
				return null; // completed synchronously
			}
			
			var errorCode = Marshal.GetLastPInvokeError();
			if (errorCode != ERROR_IO_PENDING)
			{
				return GetWriteError(errorCode, ref mustClose);
			}
			
			if (!Kernel32.GetOverlappedResultEx(_handle, ref ol, out var bytesWritten, INFINITE, false))
			{
				return GetWriteError(Marshal.GetLastPInvokeError(), ref mustClose);
			}
			
			Debug.Assert(bytesWritten == Info.OutputReportByteLength);
			return null;
		}
		finally
		{
			if (mustClose) Close();
		}
	}
	
	protected override HidError? InternalRead(Span<byte> buffer, int timeout)
	{
		Debug.Assert(buffer.Length == Info.InputReportByteLength);
		
		bool mustClose = false;
		
		if (!_isOpen)
		{
			if (!TryOpen(DeviceAccess.Read, out var error)) return error;
			mustClose = true;
		}
		
		Debug.Assert(_handle is { IsInvalid: false, IsClosed: false });
		
		try
		{
			var ol = new Structs.OVERLAPPED(_manualEvent.DangerousGetHandle());
			
			if (Kernel32.ReadFile(_handle, ref MemoryMarshal.GetReference(buffer), Info.InputReportByteLength, out _, ref ol))
			{
				return null; // completed synchronously
			}
			
			int errorCode = Marshal.GetLastPInvokeError();
			if (errorCode != ERROR_IO_PENDING)
			{
				return GetReadError(errorCode, ref mustClose);
			}
			
			if (Kernel32.GetOverlappedResultEx(_handle, ref ol, out var bytesRead, unchecked((UInt32)timeout), false))
			{
				Debug.Assert(bytesRead == Info.InputReportByteLength);
				return null;
			}
			
			if ((errorCode = Marshal.GetLastPInvokeError())
			    is not WAIT_TIMEOUT          // timed out
			    and not ERROR_IO_INCOMPLETE) // 'timeout' was 0 and the operation is still in progress
			{
				return GetReadError(errorCode, ref mustClose);
			}
			
			if (Kernel32.CancelIoEx(_handle, ref ol))
			{
				return Errors.Timeout;
			}
			
			switch (errorCode = Marshal.GetLastPInvokeError())
			{
			case ERROR_NOT_FOUND: 
				// The IO operation had already been finished by the time we tried to cancel it.
				// Let's make another try and get the result.
				if (!Kernel32.GetOverlappedResultEx(_handle, ref ol, out bytesRead, 0, false))
				{
					return GetReadError(Marshal.GetLastPInvokeError(), ref mustClose);
				}
				
				Debug.Assert(bytesRead == Info.InputReportByteLength);
				return null;
			
			default:
				return new HidError(ErrorKind.Other, $"Failed to cancel read IO: {Helpers.GetFormattedErrorMessage(errorCode)}");
			}
		}
		finally
		{
			if (mustClose) Close();
		}
	}
	
	private static HidError GetWriteError(int errorCode, ref bool mustClose)
	{
		switch (errorCode)
		{
		case ERROR_DEVICE_NOT_CONNECTED: // device got disconnected
			mustClose = true; // close the handle so the caller doesn't have to
			return Errors.DeviceNotConnected;
		
		default:
			return new HidError(ErrorKind.Other, $"Failed to write: {Helpers.GetFormattedErrorMessage(errorCode)}");
		}
	}
	
	// TODO: combine with GetWriteError?
	private static HidError GetReadError(int errorCode, ref bool mustClose)
	{
		switch (errorCode)
		{
		case ERROR_DEVICE_NOT_CONNECTED: // device got disconnected
			mustClose = true; // close the handle so the caller doesn't have to
			return Errors.DeviceNotConnected;
		
		default:
			return new HidError(ErrorKind.Other, $"Failed to read: {Helpers.GetFormattedErrorMessage(errorCode)}");
		}
	}
}