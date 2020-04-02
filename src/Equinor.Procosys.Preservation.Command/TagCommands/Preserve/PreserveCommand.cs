using Equinor.Procosys.Preservation.Domain.ProjectAccess;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve
{
    [PathToProject(PathToProjectType.Tag, nameof(TagId))]
    public class PreserveCommand : IRequest<Result<Unit>>
    {
        public PreserveCommand(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
