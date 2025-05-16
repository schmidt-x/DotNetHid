using System;
using System.Runtime.InteropServices;
using DotNetHid.Platform.Windows.SafeHandles;
using static DotNetHid.Platform.Windows.Interop.Structs;

namespace DotNetHid.Platform.Windows.Interop;

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
	
	[LibraryImport(LibName, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	internal static partial HANDLE CreateEventW(
		IntPtr lpEventAttributes,
		[MarshalAs(UnmanagedType.Bool)] bool bManualReset,
		[MarshalAs(UnmanagedType.Bool)] bool bInitialState,
		string? lpName);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool WriteFile(
		HANDLE hFile, ref byte lpBuffer, UInt32 nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, ref OVERLAPPED lpOverlapped);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool ReadFile(
		HANDLE hFile, ref byte lpBuffer, UInt32 nNumberOfBytesToRead, out UInt32 lpNumberOfBytesRead, ref OVERLAPPED lpOverlapped);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool GetOverlappedResultEx(
		HANDLE hFile,
		ref OVERLAPPED lpOverlapped,
		out UInt32 lpNumberOfBytesTransferred,
		UInt32 dwMilliseconds,
		[MarshalAs(UnmanagedType.Bool)] bool bAlertable);
	
	[LibraryImport(LibName, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool CancelIoEx(HANDLE hFile, ref OVERLAPPED lpOverlapped);
}
