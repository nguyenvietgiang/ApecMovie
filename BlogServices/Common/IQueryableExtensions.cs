using System.Linq.Expressions;

namespace BlogServices.Common
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }
    }
}
