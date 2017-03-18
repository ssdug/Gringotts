using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Wiz.Gringotts.UIWeb.Models;

namespace Wiz.Gringotts.UIWeb.Data
{
    public class Lookup<T> : ILookup<T> where T:class, IAmLookupItem
    {
        private const int MaxItems = 100;

        private readonly ApplicationDbContext context;

        public Lookup(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<T> All
        {
            get
            {
                return context.Set<T>()
                    .Take(MaxItems)
                    .ToArray();
            }
        }
    }
}