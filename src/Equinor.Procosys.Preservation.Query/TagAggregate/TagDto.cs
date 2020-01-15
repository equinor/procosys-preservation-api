using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class TagDto
    {
        public TagDto(int id, string tagNo, int stepId, IEnumerable<RequirementDto> requirements)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }
            
            Id = id;
            TagNo = tagNo;
            StepId = stepId;
            Requirements = requirements;
        }
        public int Id { get; }
        public string TagNo { get; }
        public int StepId { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
    }
}
