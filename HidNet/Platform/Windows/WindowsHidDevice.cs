namespace HidNet.Platform.Windows;

internal class WindowsHidDevice : HidDevice
{
	private readonly HidDeviceInfo _deviceInfo;
	private readonly bool _outputWarnings;

	internal WindowsHidDevice(HidDeviceInfo deviceInfo, bool outputWarnings)
	{
		_deviceInfo = deviceInfo;
		_outputWarnings = outputWarnings;
	}
}