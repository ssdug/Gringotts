using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace Wiz.Gringotts.UIWeb.Models.ReceiptSources
{
    public class ReceiptSourceDetails
    {
        public ReceiptSource ReceiptSource { get; set; }
    }

    public class ReceiptSourceSearchResult : IPagedSearchResult<ReceiptSource>
    {
        public SearchPager Pager { get; set; }
        public IPagedList<ReceiptSource> Items { get; set; }
    }
}