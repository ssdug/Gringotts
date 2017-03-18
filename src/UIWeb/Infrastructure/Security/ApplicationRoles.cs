using System.Collections.Generic;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Security
{
    public static class ApplicationRoles
    {
        public static IEnumerable<string> All = new[]
        {
            SystemAdministrator, Developer, Auditor, ReadOnly, Fiscal, FiscalSupervisor, FiscalCoordinator
        }; 

        public const string SystemAdministrator = "System Administrator";
        public const string Developer = "Developer";

        public const string Auditor = "Auditor";

        public const string ReadOnly = "Read Only";
        public const string Fiscal = "Fiscal";
        public const string FiscalSupervisor = "Fiscal Supervisor";
        public const string FiscalCoordinator = "Fiscal Coordinator";
    }
}