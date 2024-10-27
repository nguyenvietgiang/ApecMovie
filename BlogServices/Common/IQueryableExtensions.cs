using System.Linq.Expressions;


// chứa các phương thức mở rộng tiện cho việc thao tác với dữ liệu
namespace BlogServices.Common
{
    public static class IQueryableExtensions
    {
        // lọc có điều kiện
        public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }
        // sắp xếp có điều kiện
        public static IQueryable<T> OrderByIf<T, TKey>(
    this IQueryable<T> source,
    bool condition,
    Expression<Func<T, TKey>> keySelector)
        {
            return condition ? source.OrderBy(keySelector) : source;
        }

        // lọc trùng
        public static IQueryable<T> DistinctByCustom<T, TKey>(
    this IQueryable<T> source,
    Expression<Func<T, TKey>> keySelector)
        {
            return source.GroupBy(keySelector).Select(g => g.First());
        }


        public static IQueryable<T> Paginate<T>(
    this IQueryable<T> source,
    int pageNumber,
    int pageSize)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

    }
}
