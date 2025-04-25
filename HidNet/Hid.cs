using System;
using System.Collections.Generic;

namespace HidNet;

public static class Hid
{
	private static readonly HidDevices Instance;

	static Hid()
	{
		if (OperatingSystem.IsWindows())
		{
			throw new NotImplementedException("Windows system is not implemented");
		}
		else if (OperatingSystem.IsLinux())
		{
			throw new NotImplementedException("Linux system is not implemented");
		}
		else
		{
			throw new PlatformNotSupportedException();
		}
	}
	
	/// <summary>
	/// Enumerates all connected HID devices, optionally filtering them by the specified criteria.
	/// </summary>
	/// <param name="vendorId">Device vendor ID.</param>
	/// <param name="productId">Device product ID.</param>
	/// <param name="usagePage">HID Usage Page.</param>
	/// <param name="usageId">HID Usage ID.</param>
	/// <returns>A sequence of containers with device-specific information.
	/// Used by <see cref="Create"/> to instantiate the device for communication.</returns>
	public static IEnumerable<HidDeviceInfo> Enumerate(
		UInt16? vendorId = null, UInt16? productId = null, UInt16? usagePage = null, UInt16? usageId = null)
	{
		return Instance.Enumerate(vendorId, productId, usagePage, usageId);
	}
	
	/// <summary>
	/// Creates an instance of a HID device for communication.
	/// </summary>
	/// <param name="deviceInfo">Device-specific information returned by <see cref="Enumerate"/>.</param>
	/// <returns>A <see cref="HidDevice"/> instance representing the device.</returns>
	public static HidDevice Create(HidDeviceInfo deviceInfo)
	{
		ArgumentNullException.ThrowIfNull(deviceInfo, nameof(deviceInfo));
		return Instance.Create(deviceInfo);
	}
}
