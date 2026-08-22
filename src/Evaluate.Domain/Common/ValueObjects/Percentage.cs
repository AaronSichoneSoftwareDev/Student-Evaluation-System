namespace Evaluate.Domain.Common.ValueObjects;

/// <summary>
/// A percentage value clamped to the valid 0-100 range, so the invariant is
/// enforced once here instead of being re-checked at every call site.
/// </summary>
public readonly record struct Percentage
{
    public decimal Value { get; }

    private Percentage(decimal value) => Value = value;

    public static Percentage Create(decimal value) => new(Math.Clamp(value, 0m, 100m));

    public static implicit operator decimal(Percentage percentage) => percentage.Value;

    public override string ToString() => $"{Value:0.##}%";
}
