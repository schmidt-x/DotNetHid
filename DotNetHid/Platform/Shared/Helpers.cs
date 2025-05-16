using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotNetHid.Platform.Shared;

internal static class Helpers
{
	internal static string GetFormattedErrorMessage(int errorCode) => $"({errorCode}) {Marshal.GetPInvokeErrorMessage(errorCode)}";
	
	internal static void Log(
		string errorMessage,
		[CallerMemberName] string? memberName = null,
		[CallerFilePath] string? filePath = null,
		[CallerLineNumber] int lineNumber = 0)
	{
		var timeStamp = DateTime.Now.ToString("HH:mm:ss");
		var objectName = Path.GetFileNameWithoutExtension(filePath);
		Console.WriteLine($"[{timeStamp} {objectName}:{memberName}:{lineNumber}] {errorMessage}");
	}
}