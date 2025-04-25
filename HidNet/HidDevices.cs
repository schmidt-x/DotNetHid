using System;
using System.Collections.Generic;

namespace HidNet;

internal abstract class HidDevices
{
	internal abstract IEnumerable<HidDeviceInfo> Enumerate(
		UInt16? vendorId = null, UInt16? productId = null, UInt16? usagePage = null, UInt16? usageId = null);
	
	internal abstract HidDevice Create(HidDeviceInfo deviceInfo);
}