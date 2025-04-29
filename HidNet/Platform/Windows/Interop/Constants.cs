using System;

namespace HidNet.Platform.Windows.Interop;

// resharper disable InconsistentNaming
internal static class Constants
{
	#region setupapi.h
	
	internal const int DIGCF_PRESENT         = 0x02;
	internal const int DIGCF_DEVICEINTERFACE = 0x10;
	
	#endregion
	
	#region winerror.h
	
	internal const int ERROR_FILE_NOT_FOUND      = 2;
	internal const int ERROR_SHARING_VIOLATION   = 32;
	internal const int ERROR_INSUFFICIENT_BUFFER = 122;
	internal const int ERROR_NO_MORE_ITEMS       = 259;
	internal const int ERROR_OPERATION_ABORTED   = 995;
	
	#endregion
	
	#region winnt.h
	
	internal const UInt32 FILE_SHARE_READ  = 0x01;
	internal const UInt32 FILE_SHARE_WRITE = 0x02;
	
	internal const UInt32 GENERIC_WRITE = 0x40000000; 
	internal const UInt32 GENERIC_READ  = 0x80000000;
	
	#endregion
	
	#region fileapi.h
	
	internal const UInt32 OPEN_EXISTING = 3;
	
	#endregion
	
	#region hidpi.h
	
	internal const UInt32 HIDP_STATUS_SUCCESS = 0x00110000;
	
	#endregion
	
	#region winbase.h
	
	internal const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;
	
	#endregion
}