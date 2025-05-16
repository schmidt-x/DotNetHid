using DotNetHid.Platform.Windows.Interop;
using Microsoft.Win32.SafeHandles;

namespace DotNetHid.Platform.Windows.SafeHandles;

internal class DeviceInfoSetHandle : SafeHandleMinusOneIsInvalid
{
	public DeviceInfoSetHandle() : base(true)
	{ }

	protected override bool ReleaseHandle() => SetupApi.SetupDiDestroyDeviceInfoList(handle);
}