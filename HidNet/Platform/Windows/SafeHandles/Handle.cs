using HidNet.Platform.Windows.Interop;
using Microsoft.Win32.SafeHandles;

namespace HidNet.Platform.Windows.SafeHandles;

// Resharper disable InconsistentNaming
internal class HANDLE : SafeHandleMinusOneIsInvalid
{
	public HANDLE() : base(true)
	{ }
	
	protected override bool ReleaseHandle() => Kernel32.CloseHandle(handle);
}