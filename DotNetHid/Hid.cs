using System;
using System.Collections.Generic;
using DotNetHid.Platform.Windows;

namespace DotNetHid;

/// <summary>
/// An entry point for discovering devices and creating instances for communication.
/// </summary>
public static class Hid
{
	private static readonly HidDevices Instance;
	
	/// <summary>
	/// Enables or disables warning output to the console. Useful for diagnosing issues such as a connected device is not
	/// included in the enumeration list, often because it couldn't be opened for metadata retrieval — for example,
	/// if another process had it open with exclusive sharing rights.
	/// Must be set before calling <see cref="Enumerate"/> or <see cref="Create"/> to take effect. 
	/// </summary>
	public static bool OutputWarnings
	{
		get => Instance.OutputWarnings;
		set => Instance.OutputWarnings = value;
	}
	
	static Hid()
	{
		if (OperatingSystem.IsWindows())
		{
			Instance = new WindowsHidDevices();
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
	/// <returns>A sequence of <see cref="HidDeviceInfo"/> elements with device-specific information.
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
		ArgumentNullException.ThrowIfNull(deviceInfo);
		return Instance.Create(deviceInfo);
	}
}
