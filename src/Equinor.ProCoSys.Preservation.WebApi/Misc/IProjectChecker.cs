using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public interface IProjectChecker
    {
        Task EnsureValidProjectAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IBaseRequest;
    }
}
