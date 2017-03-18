using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;
using MediatR;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;

namespace Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory
{
    public class ValidateActiveDirectoryGroupCommand : IAsyncRequest<ICommandResult>
    {
        public string GroupName { get; private set; }
        public bool AllowApplicationGroups { get; private set; }

        public ValidateActiveDirectoryGroupCommand(string groupName, bool allowApplicationGroups = true)
        {
            GroupName = groupName;
            AllowApplicationGroups = allowApplicationGroups;
        }
    }

    public class HappyValidateActiveDirectoryGroupCommandHandler : IAsyncRequestHandler<ValidateActiveDirectoryGroupCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        public HappyValidateActiveDirectoryGroupCommandHandler()
        { }

        public Task<ICommandResult> Handle(ValidateActiveDirectoryGroupCommand message)
        {
            Logger.Trace("Handle::{0}", message.GroupName);

            return Task.FromResult(new SuccessResult(true) as ICommandResult);
        }
    }

    public class ValidateActiveDirectoryGroupCommandHandler : IAsyncRequestHandler<ValidateActiveDirectoryGroupCommand, ICommandResult>, IDisable
    {
        public ILogger Logger { get; set; }

        private readonly ILdapProvider ldapProvider;

        public ValidateActiveDirectoryGroupCommandHandler(ILdapProvider ldapProvider)
        {
            this.ldapProvider = ldapProvider;
        }

        public Task<ICommandResult> Handle(ValidateActiveDirectoryGroupCommand message)
        {
            Logger.Trace("Handle::{0}", message.GroupName);

            return Task.Run<ICommandResult>(() =>
            {
                if (!message.AllowApplicationGroups && GroupIsNotAllowed(message.GroupName))
                    return new FailureResult("Application Role Groups are not allowed as an Organizational Group.");

                if (ldapProvider.GroupExists(message.GroupName))
                    return new SuccessResult(true);

                return new FailureResult("Group not found in active directory.");
            });
        }

        private bool GroupIsNotAllowed(string groupName)
        {
            return ApplicationRoles.All.Contains(groupName);
        }
    }
}