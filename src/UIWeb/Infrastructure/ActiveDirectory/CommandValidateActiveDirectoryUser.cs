using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using MediatR;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;

namespace Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory
{
    public class ValidateActiveDirectoryUserCommand : IAsyncRequest<ICommandResult>
    {
        public string UserName { get; private set; }

        public ValidateActiveDirectoryUserCommand(string userName)
        {
            this.UserName = userName;
        }
    }

    public class HappyValidateActiveDirectoryUserCommandHandler : IAsyncRequestHandler<ValidateActiveDirectoryUserCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        public HappyValidateActiveDirectoryUserCommandHandler()
        { }

        public Task<ICommandResult> Handle(ValidateActiveDirectoryUserCommand message)
        {
            Logger.Trace("Handle::{0}", message.UserName);

            return Task.FromResult(new SuccessResult(true) as ICommandResult);
        }
    }

//    public class ValidateActiveDirectoryUserCommandHandler : IAsyncRequestHandler<ValidateActiveDirectoryUserCommand, ICommandResult>,IDisable
//    {
//        public ILogger Logger { get; set; }
//        private readonly ILdapProvider ldapProvider;

//        public ValidateActiveDirectoryUserCommandHandler(ILdapProvider ldapProvider)
//        {
//            this.ldapProvider = ldapProvider;
//        }

//        public Task<ICommandResult> Handle(ValidateActiveDirectoryUserCommand message)
//        {
//            Logger.Trace("Handle::{0}", message.UserName);

//            return Task.Run<ICommandResult>(() =>
//            {
//                if (ldapProvider.UserExists(message.UserName))
//                    return new SuccessResult(true);

//                return new FailureResult("User not found in active directory.");
//            });
//        }
//    }
}