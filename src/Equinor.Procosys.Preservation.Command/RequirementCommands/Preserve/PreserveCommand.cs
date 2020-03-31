using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve
{
    public class PreserveCommand : IRequest<Result<Unit>>
    {
        public PreserveCommand(string plant, int tagId, int requirementId)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            TagId = tagId;
            RequirementId = requirementId;
        }

        public string Plant { get; }
        public int TagId { get; }
        public int RequirementId { get; }
    }
}
