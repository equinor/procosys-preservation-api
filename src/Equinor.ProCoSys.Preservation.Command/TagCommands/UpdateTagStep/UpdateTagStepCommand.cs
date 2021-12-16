using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagStep
{
    public class UpdateTagStepCommand : IRequest<Result<IEnumerable<IdAndRowVersion>>>, ITagCommandRequest
    {
        public UpdateTagStepCommand(IEnumerable<IdAndRowVersion> tags, int stepId)
        {
            Tags = tags ?? new List<IdAndRowVersion>();
            StepId = stepId;
        }

        public int StepId { get; }

        public IEnumerable<IdAndRowVersion> Tags { get; }

        public int TagId
        {
            get
            {
                if (!Tags.Any())
                {
                    throw new Exception($"At least 1 {nameof(Tags)} must be given!");
                }

                return Tags.First().Id;
            }
        }
    }
}
