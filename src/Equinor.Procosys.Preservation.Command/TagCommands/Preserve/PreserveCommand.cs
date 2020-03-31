using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommand : IRequest<Result<Unit>>
    {
        public PreserveCommand(string plant, int tagId)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            TagId = tagId;
        }

        public string Plant { get; }
        public int TagId { get; }
    }
}
