using System.Collections.Generic;

namespace Wiz.Gringotts.UIWeb.Models
{
    public interface ILookup<T> where T : class,IAmLookupItem
    {
        IEnumerable<T> All { get; }
    }
}