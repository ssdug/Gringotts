namespace Wiz.Gringotts.UIWeb.Infrastructure.Commands
{
    public class SuccessResult : ICommandResult
    {
        public SuccessResult() { }
        public SuccessResult(object result)
        {
            Result = result;
        }

        public bool IsSuccess { get { return true; } }
        public bool IsFailure { get { return false; } }
        public object Result { get; set; }
    }
}