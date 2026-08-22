namespace Evaluate.Application.Common.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> Apply<T>(this IQueryable<T> query, ISpecification<T> specification) =>
        query.Where(specification.Criteria);
}
