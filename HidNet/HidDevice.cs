using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
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
	/// A <see cref="DeviceAccess"/> value that specifies the operations that can be performed on the device.
	/// </param>
	/// <returns>A <see cref="HidError"/> describing the error if the device fails to open; otherwise, null.</returns>
	public abstract HidError? Open(DeviceAccess access);
	
	/// <summary>
	/// Opens the device with read/write access mode.
	/// </summary>
	/// <returns>A <see cref="HidError"/> describing the error if the device fails to open; otherwise, null.</returns>
	public HidError? Open() => Open(DeviceAccess.ReadWrite);
	
	/// <summary>
	/// Attempts to open the device with the specified access mode.
	/// </summary>
	/// <param name="access">
	/// A <see cref="DeviceAccess"/> value that specifies the operations that can be performed on the device.
	/// </param>
	/// <param name="error">
	/// When this method returns false, contains a <see cref="HidError"/> describing the error; otherwise, null.
	/// </param>
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
	/// When this method returns false, contains a <see cref="HidError"/> describing the error; otherwise, null.
	/// </param>
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
	/// <see cref="HidDeviceInfo.OutputReportByteLength"/>-1. If this limit is exceeded, the extra bytes are trimmed.
	/// </param>
	/// <returns>A <see cref="HidError"/> describing the error if the operation fails; otherwise, null.</returns>
	public abstract HidError? Write(ReadOnlySpan<byte> output);
	
	/// <summary>
	/// Tries to send an output report to the device.
	/// </summary>
	/// <param name="output">
	/// Data to send. The Report ID byte is prepended automatically, so the actual report length limit is
	/// <see cref="HidDeviceInfo.OutputReportByteLength"/>-1. If this limit is exceeded, the extra bytes are trimmed.
	/// </param>
	/// <param name="error">
	/// When this method returns false, contains a <see cref="HidError"/> describing the error; otherwise, null.
	/// </param>
	/// <returns>True if succeeded; false otherwise.</returns>
	public bool TryWrite(ReadOnlySpan<byte> output, [MaybeNullWhen(true)] out HidError error)
	{
		error = Write(output);
		return error is null;
	}
	
	/// <summary>
	/// Reads an input report from the device, waiting up to the specified timeout period.
	/// </summary>
	/// <param name="timeout">
	/// The maximum time to wait for the input report, in milliseconds. If the operation times out, the returned
	/// <see cref="HidError"/> will have its member <see cref="HidError.Kind"/> set to <see cref="ErrorKind.Timeout"/>.
	/// Specify 0 to immediately return if no report is available. To wait infinitely until a report is received, either
	/// specify <see cref="Timeout.Infinite"/> (-1) or use the parameterless overload <see cref="Read()"/>.
	/// </param>
	/// <returns>
	/// <see cref="Result{T,TError}"/> with either <see cref="byte"/>[] (if succeeded) or <see cref="HidError"/> (if failed).
	/// The Report ID byte is omitted, so the length of the returned <see cref="byte"/>[] is always equal to
	/// <see cref="HidDeviceInfo.InputReportByteLength"/>-1.
	/// </returns>
	public abstract Result<byte[], HidError> Read(int timeout);
	
	/// <summary>
	/// Reads an input report from the device, waiting until the report is received. To specify a maximum wait time,
	/// use the <see cref="Read(int)"/> overload.
	/// </summary>
	/// <returns>
	/// <see cref="Result{T,TError}"/> with either <see cref="byte"/>[] (if succeeded) or <see cref="HidError"/> (if failed).
	/// The Report ID byte is omitted, so the length of the returned <see cref="byte"/>[] is always equal to
	/// <see cref="HidDeviceInfo.InputReportByteLength"/>-1.
	/// </returns>
	public Result<byte[], HidError> Read() => Read(Timeout.Infinite);
	
	// None of the derived classes are intended to implement finalizers, therefore,
	// calling GC.SuppressFinalizer is unnecessary.
#pragma warning disable CA1816
	public void Dispose() => Close();
#pragma warning restore CA1816
}