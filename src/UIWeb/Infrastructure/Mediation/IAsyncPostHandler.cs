using System.Threading.Tasks;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Mediation
{
    public interface IAsyncPostRequestHandler<in TRequest, in TResponse>
    {
        Task Handle(TRequest command, TResponse response);
    }
}