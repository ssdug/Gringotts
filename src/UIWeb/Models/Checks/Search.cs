using System;
using System.Linq;
using System.Linq.Expressions;
using Wiz.Gringotts.UIWeb.Data;

namespace Wiz.Gringotts.UIWeb.Models.Checks
{
    public class SearchCheck: ISearch<Check>
    {
        private readonly ApplicationDbContext context;

        public SearchCheck(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Check> All()
        {
            return context.Checks;
        }

        public IQueryable<Check> GetById(int id)
        {
            return All()
                .Where(FilterById(id));
        }
        private Expression<Func<Check, bool>> FilterById(int id)
        {
            return check => check.Id == id;
        }
        public IQueryable<Check> GetBySearch(SearchPager searchPager)
        {
            return All()
                .Where(FilterBySearch(searchPager.Search));
        }

        private Expression<Func<Check, bool>> FilterBySearch(string search)
        {
            Expression<Func<Check, bool>> noSearchExpression =
                check => true;

            Expression<Func<Check, bool>> searchExpression =
                check => check.PaidTo.StartsWith(search)                     
                          .Equals(search);

            return string.IsNullOrWhiteSpace(search) ? noSearchExpression : searchExpression;
        }
    }
}