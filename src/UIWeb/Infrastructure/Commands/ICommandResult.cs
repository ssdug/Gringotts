namespace Wiz.Gringotts.UIWeb.Infrastructure.Commands
{
    public interface ICommandResult
    {
        bool IsSuccess { get; }
        bool IsFailure { get; }
        object Result { get; set; }
    }
}