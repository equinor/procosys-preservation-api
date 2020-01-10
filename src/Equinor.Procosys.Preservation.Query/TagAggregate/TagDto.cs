using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class TagDto
    {
        public TagDto(int id, IEnumerable<RequirementDto> requirements)
        {
            Id = id;
            Requirements = requirements;
        }
        public int Id { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
    }
}
