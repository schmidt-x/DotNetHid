namespace DotNetHid.Enums;

/// <summary>
/// Represents the kinds of errors that can occur when accessing or interacting with a HID device.
/// </summary>
public enum ErrorKind
{
	/// <summary>
	/// The device is not connected.
	/// </summary>
	DeviceNotConnected,
	
	/// <summary>
	/// The device is currently in use by another process and cannot be accessed.
	/// </summary>
	DeviceInUse,
	
	/// <summary>
	/// One or more of the specified arguments are invalid.
	/// </summary>
	InvalidArgument,
	
	/// <summary>
	/// Operation timed out.
	/// </summary>
	Timeout,
	
	/// <summary>
	/// An unspecified error.
	/// </summary>
	Other
}