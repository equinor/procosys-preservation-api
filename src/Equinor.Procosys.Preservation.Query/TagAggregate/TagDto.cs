using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class TagDto
    {
        public TagDto(int id, string tagNo, int stepId, PreservationStatus status, IEnumerable<RequirementDto> requirements, bool needUserInput)
        {
            Id = id;
            TagNo = tagNo;
            Status = status;
            StepId = stepId;
            Requirements = requirements ?? throw new ArgumentNullException(nameof(requirements));
            NeedUserInput = needUserInput;
        }
        public int Id { get; }
        public string TagNo { get; }
        public int StepId { get; }
        public PreservationStatus Status { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
        public bool NeedUserInput { get; }
    }
}
