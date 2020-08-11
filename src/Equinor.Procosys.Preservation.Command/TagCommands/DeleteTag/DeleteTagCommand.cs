using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.DeleteTag
{
    public class DeleteTagCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public DeleteTagCommand(int tagId, string projectName, string rowVersion)
        {
            TagId = tagId;
            ProjectName = projectName;
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public string ProjectName { get; }
        public string RowVersion { get; }
    }
}
