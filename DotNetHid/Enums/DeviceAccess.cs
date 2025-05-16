using System;

namespace DotNetHid.Enums;

/// <summary>
/// Defines constants for read, write, or read/write access to a device.
/// </summary>
[Flags]
public enum DeviceAccess
{
	Read = 1,
	Write = 2,
	ReadWrite = Read | Write
}