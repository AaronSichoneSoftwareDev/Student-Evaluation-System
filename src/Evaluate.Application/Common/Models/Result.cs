namespace Evaluate.Application.Common.Models;

/// <summary>Result pattern for *expected* business-rule outcomes (e.g. "student already
/// enrolled this year") — as opposed to malformed input, which FluentValidation rejects
/// before a handler ever runs by throwing <see cref="Exceptions.ValidationException"/>.</summary>
public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public bool Succeeded { get; }

    public string[] Errors { get; }

    public static Result Success() => new(true, []);

    public static Result Failure(IEnumerable<string> errors) => new(false, errors);

    public static Result Failure(string error) => new(false, [error]);
}

public class Result<T> : Result
{
    private Result(bool succeeded, T? value, IEnumerable<string> errors) : base(succeeded, errors) => Value = value;

    public T? Value { get; }

    public static Result<T> Success(T value) => new(true, value, []);

    public static new Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);

    public static new Result<T> Failure(string error) => new(false, default, [error]);
}
