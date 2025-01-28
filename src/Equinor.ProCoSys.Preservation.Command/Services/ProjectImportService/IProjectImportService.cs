using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Services.ProjectImportService;

public interface IProjectImportService
{
    Task<Project> ImportProjectAsync(string projectName);
}
