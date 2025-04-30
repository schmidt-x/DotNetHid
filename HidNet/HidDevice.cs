using System;

namespace HidNet;

/// <summary>
/// Represents a HID device.
/// </summary>
public abstract class HidDevice : IDisposable
{
	/// <summary>
	/// Opens the device for communication.
	/// </summary>
	/// <returns>A <see cref="HidError"/> instance describing the error if the operation fails; otherwise, null.</returns>
	public abstract HidError? Open();
	
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