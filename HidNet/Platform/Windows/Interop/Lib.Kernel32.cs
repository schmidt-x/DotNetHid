using System;
using System.Runtime.InteropServices;
using HidNet.Platform.Windows.SafeHandles;

namespace HidNet.Platform.Windows.Interop;

internal static partial class Kernel32
{
	private const string LibName = nameof(Kernel32);
	
	[LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
	internal static partial HANDLE CreateFileW(
		string lpFileName,
		UInt32 dwDesiredAccess,
		UInt32 dwShareMode,
		IntPtr lpSecurityAttributes,
		UInt32 dwCreationDisposition,
		UInt32 dwFlagsAndAttributes,
		IntPtr hTemplateFile);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool CloseHandle(IntPtr hObject);
}
