using System;
using System.Diagnostics.CodeAnalysis;

namespace DotNetHid;

public readonly struct Result<T, TError>
{
	private readonly T? _value;
	private readonly TError? _error;

	[MemberNotNullWhen(true, nameof(_value))]
	[MemberNotNullWhen(false, nameof(_error))]
	public bool IsSuccess { get; }
	
	public T Value => IsSuccess ? _value : throw new Exception("Attempt to access Value while IsSuccess == False");
	public TError Error => !IsSuccess ? _error : throw new Exception("Attempt to access Error while IsSuccess == True");
	
	private Result(bool isSuccess, T? value, TError? error) => (IsSuccess, _value, _error) = (isSuccess, value, error);
	
	/// <summary>
	/// Unwraps the result, indicating whether the result was successful.
	/// </summary>
	/// <param name="value">
	/// When this method returns true, contains the unwrapped success value of type <see cref="T"/>;
	/// otherwise, the default value.
	/// </param>
	/// <param name="error">
	/// When this method returns false, contains the unwrapped error of type <see cref="TError"/>;
	/// otherwise, the default value.
	/// </param>
	/// <returns>true if the result was successful; false otherwise.</returns>
	public bool IsOk([MaybeNullWhen(false)] out T value, [MaybeNullWhen(true)] out TError error)
	{
		value = _value;
		error = _error;
		return IsSuccess;
	}
	
	public static implicit operator Result<T, TError>(T value) => new(true, value, default);
	public static implicit operator Result<T, TError>(TError error) => new(false, default, error);
	
	public Result<TNewValue, TError> Map<TNewValue>(Func<T, TNewValue> map) => IsSuccess ? map(_value) : _error;
	public Result<T, TNewError> MapError<TNewError>(Func<TError, TNewError> map) => !IsSuccess ? map(_error) : _value;
	
}
