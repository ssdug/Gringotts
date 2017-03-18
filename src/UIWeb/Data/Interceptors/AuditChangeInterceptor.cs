using System;
using System.Data.Entity.Infrastructure;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;
using Wiz.Gringotts.UIWeb.Models;

namespace Wiz.Gringotts.UIWeb.Data.Interceptors
{
    public class AuditChangeInterceptor : ChangeInterceptor<IAmAuditable>
    {
        public ILogger Logger { get; set; }

        private readonly IPrincipalProvider principalProvider;

        public AuditChangeInterceptor(IPrincipalProvider principalProvider)
        {
            this.principalProvider = principalProvider;
        }

        protected override void OnBeforeInsert(DbEntityEntry entry, IAmAuditable item, InterceptionContext context)
        {
            Logger.Trace("OnBeforeInsert");

            item.Created = item.Updated = DateTime.UtcNow;
            item.CreatedBy = item.UpdatedBy = principalProvider.GetCurrent().Identity.Name;

            base.OnBeforeInsert(entry, item, context);
        }

        protected override void OnBeforeUpdate(DbEntityEntry entry, IAmAuditable item, InterceptionContext context)
        {
            Logger.Trace("OnBeforeInsert");

            item.Updated = DateTime.UtcNow;
            item.UpdatedBy = principalProvider.GetCurrent().Identity.Name;

            base.OnBeforeUpdate(entry, item, context);
        }  
    }
}