using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HidNet.Enums;
using HidNet.Platform.Shared;
using HidNet.Platform.Windows.Interop;
using HidNet.Platform.Windows.SafeHandles;
using static HidNet.Platform.Windows.Interop.Constants;

namespace HidNet.Platform.Windows;

internal class WindowsHidDevice : HidDevice
{
	private readonly bool _outputWarnings;
	
	private HANDLE? _handle;
	private bool _isOpen;

	internal WindowsHidDevice(HidDeviceInfo info, bool outputWarnings) : base(info)
	{
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
}