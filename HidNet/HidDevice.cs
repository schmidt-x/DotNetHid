using System;
using System.Diagnostics.CodeAnalysis;
using HidNet.Enums;

namespace HidNet;

/// <summary>
/// Represents a HID device.
/// </summary>
public abstract class HidDevice : IDisposable
{
	/// <summary>
	/// Device description.
	/// </summary>
	public HidDeviceInfo Info { get; }

	protected HidDevice(HidDeviceInfo info) => Info = info;
	
	/// <summary>
	/// Opens the device with the specified access mode.
	/// </summary>
	/// <param name="access">
	/// A <see cref="DeviceAccess"/> value that specifies the operations that can be performed on the device.</param>
	/// <returns>A <see cref="HidError"/> instance describing the error if the device fails to open; otherwise, null.</returns>
	public abstract HidError? Open(DeviceAccess access);
	
	/// <summary>
	/// Opens the device with read/write access mode.
	/// </summary>
	/// <returns>A <see cref="HidError"/> instance describing the error if the device fails to open; otherwise, null.</returns>
	public HidError? Open() => Open(DeviceAccess.ReadWrite);
	
	/// <summary>
	/// Attempts to open the device with the specified access mode.
	/// </summary>
	/// <param name="access">
	/// A <see cref="DeviceAccess"/> value that specifies the operations that can be performed on the device.</param>
	/// <param name="error">
	/// When this method returns, contains <see cref="HidError"/> describing the error if the device failed to open;
	/// otherwise, null.</param>
	/// <returns>True if succeeded; false otherwise.</returns>
	public bool TryOpen(DeviceAccess access, [MaybeNullWhen(true)] out HidError error)
	{
		error = Open(access);
		return error is null;
	}
	
	/// <summary>
	/// Attempts to open the device with read/write access mode.
	/// </summary>
	/// <param name="error">
	/// When this method returns, contains <see cref="HidError"/> describing the error if the device failed to open;
	/// otherwise, null.</param>
	/// <returns>True if succeeded; false otherwise.</returns>
	public bool TryOpen([MaybeNullWhen(true)] out HidError error)
	{
		error = Open();
		return error is null;
	}
	
	/// <summary>
	/// Closes the device.
	/// </summary>
	public abstract void Close();
	
	/// <summary>
	/// Sends an output report to the device.
	/// </summary>
	/// <param name="output">
	/// Data to send. The Report ID byte is prepended automatically, so the actual report length limit is
	/// <c>.Info.OutputReportByteLength-1</c>. If this limit is exceeded, the extra bytes are trimmed. </param>
	/// <returns>A <see cref="HidError"/> instance describing the error if the operation fails; otherwise, null.</returns>
	public abstract HidError? Write(ReadOnlySpan<byte> output);
	
	/// <summary>
	/// Tries to send an output report to the device.
	/// </summary>
	/// <param name="output">
	/// Data to send. The Report ID byte is prepended automatically, so the actual report length limit is
	/// <c>.Info.OutputReportByteLength-1</c>. If this limit is exceeded, the extra bytes are trimmed. </param>
	/// <param name="error">
	/// When this method returns, contains <see cref="HidError"/> describing the error if the operation failed;
	/// otherwise, null.</param>
	/// <returns>True if succeeded; false otherwise.</returns>
	public bool TryWrite(ReadOnlySpan<byte> output, [MaybeNullWhen(true)] out HidError error)
	{
		error = Write(output);
		return error is null;
	}
	
	// None of the derived classes are intended to implement finalizers, therefore,
	// calling GC.SuppressFinalizer is unnecessary.
#pragma warning disable CA1816
	public void Dispose() => Close();
#pragma warning restore CA1816
}