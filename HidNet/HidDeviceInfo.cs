using System;

namespace HidNet;

public record HidDeviceInfo(
	UInt16 VendorId,
	UInt16 ProductId,
	UInt16 UsagePage,
	UInt16 UsageId,
	
	string Path,
	string ManufacturerString,
	string ProductString,
	
	UInt16 InputReportByteLength,
	UInt16 OutputReportByteLength);
