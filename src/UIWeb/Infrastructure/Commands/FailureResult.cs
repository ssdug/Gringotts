namespace Wiz.Gringotts.UIWeb.Infrastructure.Commands
{
    public class FailureResult : ICommandResult
    {
        public FailureResult(string failureMessage)
        {
            Result = failureMessage;
        }

        public bool IsSuccess { get { return false; } }
        public bool IsFailure { get { return true; } }
        public object Result { get; set; }
    }
}