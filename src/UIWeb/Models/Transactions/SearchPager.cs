using System;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class TransactionsSearchPager : SearchPager
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string FromDateDisplay
        {
            get { return FromDate.HasValue ? FromDate.Value.ToShortDateString() : string.Empty; }
        }

        public string ToDateDisplay
        {
            get { return ToDate.HasValue ? ToDate.Value.ToShortDateString() : string.Empty; }
        }

        public override string ToString()
        {
            return "search={0}&FromDate={1}&ToDate={2}".FormatWith(Search, FromDateDisplay, ToDateDisplay);
        }
    }
}