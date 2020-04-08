using System.Threading.Tasks;
using MediatR;

namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public interface IProjectAccessValidator
    {
        Task<bool> ValidateAsync<TRequest>(TRequest request) where TRequest: IBaseRequest;
    }
}
