using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DotNetHid.Platform.Shared;
using DotNetHid.Platform.Windows.Interop;
using DotNetHid.Platform.Windows.SafeHandles;
using static DotNetHid.Platform.Windows.Interop.Constants;

namespace DotNetHid.Platform.Windows;

internal class WindowsHidDevices : HidDevices
{
	internal override IEnumerable<HidDeviceInfo> Enumerate(
		UInt16? vendorId = null, UInt16? productId = null, UInt16? usagePage = null, UInt16? usageId = null)
	{
		foreach (var path in GetPathList())
		{
			int error;
			
			const UInt32 desiredAccess = 0u;
			const UInt32 shareMode = FILE_SHARE_READ | FILE_SHARE_WRITE;
			
			using HANDLE hDevice = Kernel32.CreateFileW(path, desiredAccess, shareMode, IntPtr.Zero, OPEN_EXISTING, 0u, IntPtr.Zero);
			if (hDevice.IsInvalid)
			{
				// If we failed to open the device, it maybe disconnected,
				// or opened by another process with the exclusive 'shareMode' rights.
				if (!OutputWarnings) continue;
				error = Marshal.GetLastPInvokeError();
				Helpers.Log($"Failed to open device: {Helpers.GetFormattedErrorMessage(error)} Path: {path}.");
				continue;
			}
			
			var attributes = new Structs.HIDD_ATTRIBUTES();
			if (!Interop.Hid.HidD_GetAttributes(hDevice, ref attributes))
			{
				if (!OutputWarnings) continue;
				error = Marshal.GetLastPInvokeError();
				Helpers.Log($"Failed to get attributes: {Helpers.GetFormattedErrorMessage(error)} Path: {path}.");
				continue;
			}
			
			if ((vendorId.HasValue && vendorId != attributes.VendorID) || (productId.HasValue && productId != attributes.ProductID))
				continue;
			
			if (!Interop.Hid.HidD_GetPreparsedData(hDevice, out IntPtr preparsedData))
			{
				if (!OutputWarnings) continue;
				error = Marshal.GetLastPInvokeError();
				Helpers.Log($"Failed to get preparsed data: {Helpers.GetFormattedErrorMessage(error)} Path: {path}.");
				continue;
			}
			
			Structs.HIDP_CAPS caps;
			try
			{
				if (Interop.Hid.HidP_GetCaps(preparsedData, out caps) != HIDP_STATUS_SUCCESS)
				{
					if (!OutputWarnings) continue;
					error = Marshal.GetLastPInvokeError();
					Helpers.Log($"Failed to get caps: {Helpers.GetFormattedErrorMessage(error)} Path: {path}.");
					continue;
				}
			}
			finally
			{
				if (!Interop.Hid.HidD_FreePreparsedData(preparsedData) && OutputWarnings)
				{
					error = Marshal.GetLastPInvokeError();
					Helpers.Log($"Failed to free preparsed data: {Helpers.GetFormattedErrorMessage(error)} Path: {path}.");
				}
			}
			
			if ((usagePage.HasValue && usagePage != caps.UsagePage) || (usageId.HasValue && usageId != caps.Usage))
					continue;
			
			// TODO: determine device type
			// TODO: determine size for non-USB devices
			// According to docs, the maximum string length for USB devices is 126 wide characters + 1 terminating NULL character.
			const UInt32 strBuffSize = (126 + 1) * sizeof(char);
			
			IntPtr strBuffer = Marshal.AllocHGlobal((int)strBuffSize);
			string serialNumber, manufacturerString, productString;
			
			try
			{
				// TODO: Getting serial number fails quite often. It freezes and returns after ~5 seconds getting ERROR_OPERATION_ABORTED.
				
				Marshal.WriteInt16(strBuffer, 0, 0x0000);
				if (!Interop.Hid.HidD_GetSerialNumberString(hDevice, strBuffer, strBuffSize) && OutputWarnings)
				{
					error = Marshal.GetLastPInvokeError();
					Helpers.Log($"Failed to get serial number: {Helpers.GetFormattedErrorMessage(error)} Path: {path}.");
				}
				serialNumber = Marshal.PtrToStringUni(strBuffer) ?? string.Empty;
				
				Marshal.WriteInt16(strBuffer, 0, 0x0000);
				if (!Interop.Hid.HidD_GetManufacturerString(hDevice, strBuffer, strBuffSize) && OutputWarnings)
				{
					error = Marshal.GetLastPInvokeError();
					Helpers.Log($"Failed to get manufacturer string: {Helpers.GetFormattedErrorMessage(error)} Path: {path}.");
				}
				manufacturerString = Marshal.PtrToStringUni(strBuffer) ?? string.Empty;
				
				Marshal.WriteInt16(strBuffer, 0, 0x0000);
				if (!Interop.Hid.HidD_GetProductString(hDevice, strBuffer, strBuffSize) && OutputWarnings)
				{
					error = Marshal.GetLastPInvokeError();
					Helpers.Log($"Failed to get product string: {Helpers.GetFormattedErrorMessage(error)} Path: {path}.");
				}
				productString = Marshal.PtrToStringUni(strBuffer) ?? string.Empty;
			}
			finally
			{
				Marshal.FreeHGlobal(strBuffer);
			}
			
			yield return new HidDeviceInfo(
				attributes.VendorID,
				attributes.ProductID,
				attributes.VersionNumber,
				caps.UsagePage,
				caps.Usage,
				caps.InputReportByteLength,
				caps.OutputReportByteLength,
				path,
				serialNumber,
				manufacturerString,
				productString);
		}
	}

	internal override HidDevice Create(HidDeviceInfo deviceInfo) => new WindowsHidDevice(deviceInfo, OutputWarnings);

	private List<string> GetPathList()
	{
		int error;
		Interop.Hid.HidD_GetHidGuid(out var hidGuid);
		
		const UInt32 flags = DIGCF_PRESENT | DIGCF_DEVICEINTERFACE;
		using DeviceInfoSetHandle hDevInfo = SetupApi.SetupDiGetClassDevsW(ref hidGuid, IntPtr.Zero, IntPtr.Zero, flags);
		if (hDevInfo.IsInvalid)
		{
			if (!OutputWarnings) return [];
			error = Marshal.GetLastPInvokeError();
			Helpers.Log($"Failed to load Device Information set: {Helpers.GetFormattedErrorMessage(error)}");
			return [];
		}
		
		List<string> paths = [];
		
		var interfaceData = new Structs.SP_DEVICE_INTERFACE_DATA();
		var index = 0u;
		
		while (SetupApi.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref hidGuid, index++, ref interfaceData))
		{
			_ = SetupApi.SetupDiGetDeviceInterfaceDetailW(hDevInfo, ref interfaceData, IntPtr.Zero, 0u, out var requiredSize, IntPtr.Zero);
			
			if ((error = Marshal.GetLastPInvokeError()) != ERROR_INSUFFICIENT_BUFFER)
			{
				if (!OutputWarnings) continue;
				Helpers.Log($"Failed to get device interface's required size (index {index-1}): {Helpers.GetFormattedErrorMessage(error)}");
				continue;
			}
			
			using var interfaceDetailData = new Structs.SP_DEVICE_INTERFACE_DETAIL_DATA_W(requiredSize);
			
			if (!SetupApi.SetupDiGetDeviceInterfaceDetailW(
				hDevInfo, ref interfaceData, interfaceDetailData, requiredSize, IntPtr.Zero, IntPtr.Zero))
			{
				if (!OutputWarnings) continue;
				error = Marshal.GetLastPInvokeError();
				Helpers.Log($"Failed to get device interface detail (index {index-1}): {Helpers.GetFormattedErrorMessage(error)}");
				continue;
			}
			
			paths.Add(interfaceDetailData.DevicePath);
		}

		if ((error = Marshal.GetLastPInvokeError()) != ERROR_NO_MORE_ITEMS)
		{
			Helpers.Log($"Device Interface enumeration failed (index: {index-1}): {Helpers.GetFormattedErrorMessage(error)}");
			return [];
		}
		
		return paths;
	}
}