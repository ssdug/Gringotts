using System.Threading.Tasks;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Mediation
{
    public interface IAsyncPreRequestHandler<in TRequest>
    {
        Task Handle(TRequest request);
    }
}