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

	protected HidDevice(HidDeviceInfo info)
	{
		Info = info;
	}
	
	/// <summary>
	/// Opens the device with the specified access mode.
	/// </summary>
	/// <param name="access">A <see cref="DeviceAccess"/> value that specifies the operations that can be performed on the device.</param>
	/// <returns>A <see cref="HidError"/> instance describing the error if the operation fails; otherwise, null.</returns>
	public abstract HidError? Open(DeviceAccess access);
	
	/// <summary>
	/// Opens the device with read/write access mode.
	/// </summary>
	/// <returns>A <see cref="HidError"/> instance describing the error if the operation fails; otherwise, null.</returns>
	public HidError? Open() => Open(DeviceAccess.ReadWrite);
	
	/// <summary>
	/// Attempts to open the device with the specified access mode.
	/// </summary>
	/// <param name="access">A <see cref="DeviceAccess"/> value that specifies the operations that can be performed on the device.</param>
	/// <param name="error">When this method returns, contains <see cref="HidError"/> describing the error
	/// if the device failed to open; otherwise, null.</param>
	/// <returns>True if succeeded; false otherwise.</returns>
	public bool TryOpen(DeviceAccess access, [MaybeNullWhen(true)] out HidError error)
	{
		error = Open(access);
		return error is null;
	}
	
	/// <summary>
	/// Attempts to open the device with read/write access mode.
	/// </summary>
	/// <param name="error">When this method returns, contains <see cref="HidError"/> describing the error
	/// if the device failed to open; otherwise, null.</param>
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
	
	// None of the derived classes are intended to implement finalizers, therefore,
	// calling GC.SuppressFinalizer is unnecessary.
#pragma warning disable CA1816
	public void Dispose() => Close();
#pragma warning restore CA1816
}