using System;
using System.Runtime.InteropServices;
using HidNet.Platform.Windows.SafeHandles;
using static HidNet.Platform.Windows.Interop.Structs;

namespace HidNet.Platform.Windows.Interop;

internal static partial class Hid
{
	private const string LibName = nameof(Hid);
	
	[LibraryImport(LibName)]
	internal static partial void HidD_GetHidGuid(out Guid hidGuid);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool HidD_GetAttributes(HANDLE hidDeviceObject, out HIDD_ATTRIBUTES attributes);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool HidD_GetPreparsedData(HANDLE hidDeviceObject, out IntPtr preparsedData);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool HidD_FreePreparsedData(IntPtr preparsedData);
	
	[LibraryImport(LibName, SetLastError = true)]
	internal static partial UInt32 HidP_GetCaps(IntPtr preparsedData, out HIDP_CAPS capabilities);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static unsafe partial bool HidD_GetSerialNumberString(HANDLE hidDeviceObject, IntPtr buffer, UInt32 bufferLength);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static unsafe partial bool HidD_GetManufacturerString(HANDLE hidDeviceObject, IntPtr buffer, UInt32 bufferLength);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static unsafe partial bool HidD_GetProductString(HANDLE hidDeviceObject, IntPtr buffer, UInt32 bufferLength);
}