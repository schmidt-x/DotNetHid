using DotNetHid.Enums;

namespace DotNetHid;

internal static class Errors
{
	internal static HidError DeviceNotConnected => new(ErrorKind.DeviceNotConnected, "Device is not connected.");
	internal static HidError DeviceInUse => new(ErrorKind.DeviceInUse, "Device is being used by another process and cannot be accessed.");
	internal static HidError Timeout => new(ErrorKind.Timeout, "Operation timed out.");
}