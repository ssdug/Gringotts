using PagedList;

namespace Wiz.Gringotts.UIWeb.Models.LivingUnits
{
    public class LivingUnitSearchResult : IPagedSearchResult<LivingUnit>
    {
        public SearchPager Pager { get; set; }
        public IPagedList<LivingUnit> Items { get; set; }
    }
}