using HidNet.Enums;

namespace HidNet;

/// <summary>
/// Error description.
/// </summary>
/// <param name="Kind">Error kind.</param>
/// <param name="Message">Error message.</param>
public record HidError(ErrorKind Kind, string Message);
