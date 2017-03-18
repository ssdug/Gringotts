using System.Linq;

namespace Wiz.Gringotts.UIWeb.Models
{
    public interface ISearch<T>
    {
        IQueryable<T> All();
        IQueryable<T> GetById(int id);

        IQueryable<T> GetBySearch(SearchPager searchPager);
    }
}