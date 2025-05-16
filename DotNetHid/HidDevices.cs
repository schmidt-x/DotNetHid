using System;
using System.Collections.Generic;

namespace DotNetHid;

internal abstract class HidDevices
{
	internal bool OutputWarnings { get; set; }

	internal abstract IEnumerable<HidDeviceInfo> Enumerate(
		UInt16? vendorId = null, UInt16? productId = null, UInt16? usagePage = null, UInt16? usageId = null);
	
	internal abstract HidDevice Create(HidDeviceInfo deviceInfo);
}