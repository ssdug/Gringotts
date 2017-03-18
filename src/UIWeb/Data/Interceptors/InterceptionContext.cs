using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Data.Interceptors
{
    public class InterceptionContext
    {
        private readonly IEnumerable<IInterceptor> interceptors;

        public InterceptionContext(IEnumerable<IInterceptor> interceptors)
        {
            this.interceptors = interceptors;
        }

        public DbContext DbContext { get; internal set; }
        public ObjectContext ObjectContext { get; internal set; }
        public ObjectStateManager ObjectStateManager { get; internal set; }
        public DbChangeTracker ChangeTracker { get; internal set; }
        public IEnumerable<DbEntityEntry> Entries { get; internal set; }
        public ILookup<EntityState, DbEntityEntry> EntriesByState { get; internal set; }

        public void Before()
        {
            interceptors.ForEach(intercept => intercept.Before(this));
        }

        public void After()
        {
            interceptors.ForEach(intercept => intercept.After(this));
        }

    }
}