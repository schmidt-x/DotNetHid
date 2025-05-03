using HidNet.Platform.Windows.Interop;
using Microsoft.Win32.SafeHandles;

namespace HidNet.Platform.Windows.SafeHandles;

// Resharper disable InconsistentNaming

internal class HANDLE
	// Some functions (such as CreateEvent) return NULL (0) as invalid handle,
	// whereas others (e.g., CreateFile) return INVALID_HANDLE_VALUE (-1).
	: SafeHandleZeroOrMinusOneIsInvalid
{
	public HANDLE() : base(true)
	{ }
	
	protected override bool ReleaseHandle() => Kernel32.CloseHandle(handle);
}