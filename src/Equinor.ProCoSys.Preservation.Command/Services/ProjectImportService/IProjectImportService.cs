using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Services.ProjectImportService;

public interface IProjectImportService
{
    Task<Project> TryGetOrImportProjectAsync(string projectName, CancellationToken cancellationToken);
}
