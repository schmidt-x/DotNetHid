using System;
using System.Runtime.InteropServices;
using HidNet.Platform.Windows.SafeHandles;
using static HidNet.Platform.Windows.Interop.Structs;

namespace HidNet.Platform.Windows.Interop;

internal static partial class SetupApi
{
	private const string LibName = nameof(SetupApi);
	
	[LibraryImport(LibName, SetLastError = true)]
	internal static partial DeviceInfoSetHandle SetupDiGetClassDevsW(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, UInt32 flags);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool SetupDiEnumDeviceInterfaces(
		DeviceInfoSetHandle deviceInfoSet,
		IntPtr deviceInfoData,
		ref Guid interfaceClassGuid,
		UInt32 memberIndex,
		ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool SetupDiGetDeviceInterfaceDetailW(
		DeviceInfoSetHandle deviceInfoSet,
		ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
		IntPtr deviceInterfaceDetailData,
		UInt32 deviceInterfaceDetailDataSize,
		out UInt32 requiredSize,
		IntPtr deviceInfoData);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool SetupDiGetDeviceInterfaceDetailW(
		DeviceInfoSetHandle deviceInfoSet,
		ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
		SP_DEVICE_INTERFACE_DETAIL_DATA_W deviceInterfaceDetailData,
		UInt32 deviceInterfaceDetailDataSize,
		UInt32 requiredSize,
		IntPtr deviceInfoData);
}