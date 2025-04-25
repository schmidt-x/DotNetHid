using HidNet.Enums;

namespace HidNet;

public record HidError(ErrorKind Kind, string Message);
