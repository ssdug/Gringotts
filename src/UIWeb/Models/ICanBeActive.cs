using System.Linq;

namespace Wiz.Gringotts.UIWeb.Models
{
    public interface ICanBeActive
    {
        bool IsActive { get; set; }
    }

    public static class ActiveModelFilters
    {
        public static IQueryable<T> FilterByIsActive<T>(this IQueryable<T> queryable, bool enabled) 
            where T : ICanBeActive
        {
            return queryable.Where(i => i.IsActive == enabled);
        } 
    }
}
