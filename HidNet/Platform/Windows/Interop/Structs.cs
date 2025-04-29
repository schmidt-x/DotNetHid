using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace HidNet.Platform.Windows.Interop;

// Resharper disable InconsistentNaming
internal static class Structs
{
	#region setupapi.h

	[StructLayout(LayoutKind.Sequential)]
	internal struct SP_DEVICE_INTERFACE_DATA
	{
		internal UInt32  cbSize;
		internal Guid    InterfaceClassGuid;
		internal UInt32  Flags;
		internal UIntPtr Reserved;
		
		public SP_DEVICE_INTERFACE_DATA() => cbSize = (UInt32)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>();
	}
	
	// Since the size of this structure is dynamic, due to the WCHAR DevicePath[ANYSIZE_ARRAY] member,
	// it is defined as a class (rather than a struct) wrapping SafeHandle
	// to abstract away the logic of memory allocation/deallocation and access to the DevicePath.
	internal class SP_DEVICE_INTERFACE_DETAIL_DATA_W : SafeHandleMinusOneIsInvalid
	{
		private string? _devicePath;
		
		internal string DevicePath =>
			_devicePath ??= Marshal.PtrToStringUni(handle + sizeof(UInt32)) ?? throw new Exception("Empty DevicePath");

		public SP_DEVICE_INTERFACE_DETAIL_DATA_W(UInt32 requiredSize) : base(true)
		{
			const UInt32 cbSize = 8; // sizeof(UInt32) + sizeof(char) + padding
			handle = Marshal.AllocHGlobal((int)requiredSize);
			Marshal.WriteInt32(handle, 0, (int)cbSize);
		}
		
		protected override bool ReleaseHandle()
		{
			Marshal.FreeHGlobal(handle);
			return true;
		}
	}

	#endregion
	
	#region hidsdi.h
	
	[StructLayout(LayoutKind.Sequential)]
	internal struct HIDD_ATTRIBUTES
	{
		internal UInt32 Size;
		internal UInt16 VendorID;
		internal UInt16 ProductID;
		internal UInt16 VersionNumber;

		public HIDD_ATTRIBUTES() => Size = (UInt32)Marshal.SizeOf<HIDD_ATTRIBUTES>();
	}
	
	#endregion
	
	#region hidpi.h
	
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	internal struct HIDP_CAPS
	{
		internal UInt16 Usage;
		internal UInt16 UsagePage;
	  internal UInt16 InputReportByteLength;
	  internal UInt16 OutputReportByteLength;
	  // TODO: add the rest of the members
	}
	
	#endregion
}