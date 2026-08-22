using System.Linq.Expressions;

namespace Evaluate.Application.Common.Specifications;

/// <summary>Specification pattern: packages a reusable, composable query filter as a
/// first-class object instead of scattering ad-hoc <c>.Where(...)</c> calls across handlers.</summary>
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}

public abstract class BaseSpecification<T>(Expression<Func<T, bool>> criteria) : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; } = criteria;
}
