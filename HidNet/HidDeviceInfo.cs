using System;

namespace HidNet;

/// <summary>
/// Device description.
/// </summary>
public class HidDeviceInfo
{
	/// <summary>
	/// HID device's vendor ID.
	/// </summary>
	public UInt16 VendorId { get; }
	
	/// <summary>
	/// HID device's product ID.
	/// </summary>
	public UInt16 ProductId { get; }
	
	/// <summary>
	/// Manufacturer's revision number for a HIDClass device.
	/// </summary>
	public UInt16 VersionNumber { get; }
	
	/// <summary>
	/// Top-level collection's usage page.
	/// </summary>
	public UInt16 UsagePage { get; }
	
	/// <summary>
	/// Top-level collection's usage ID.
	/// </summary>
	public UInt16 UsageId { get; }
	
	/// <summary>
	/// Specifies the maximum size, in bytes, of all the input reports. Includes the report ID,
	/// which is prepended to the report data. If report ID is not used, the ID value is zero.
	/// </summary>
	public UInt16 InputReportByteLength { get; }
	
	/// <summary>
	/// Specifies the maximum size, in bytes, of all the output reports. Includes the report ID,
	/// which is prepended to the report data. If report ID is not used, the ID value is zero.
	/// </summary>
	public UInt16 OutputReportByteLength { get; }
	
	/// <summary>
	/// Device interface path.
	/// </summary>
	public string DevicePath { get; }
	
	/// <summary>
	/// Serial number string.
	/// </summary>
	public string SerialNumber { get; }
	
	/// <summary>
	/// Manufacturer name.
	/// </summary>
	public string ManufacturerString { get; }
	
	/// <summary>
	/// Product name.
	/// </summary>
	public string ProductString { get; }
	
	internal HidDeviceInfo(
		UInt16 vendorId,
		UInt16 productId,
		UInt16 versionNumber,
		UInt16 usagePage,
		UInt16 usageId,
		UInt16 inputReportByteLength,
		UInt16 outputReportByteLength,
		string devicePath,
		string serialNumber,
		string manufacturerString,
		string productString)
	{
		VendorId = vendorId;
		ProductId = productId;
		VersionNumber = versionNumber;
		UsagePage = usagePage;
		UsageId = usageId;
		InputReportByteLength = inputReportByteLength;
		OutputReportByteLength = outputReportByteLength;
		DevicePath = devicePath;
		SerialNumber = serialNumber;
		ManufacturerString = manufacturerString;
		ProductString = productString;
	}
}
