using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public class GetTagRequirementInfo
    {
        public int Id { get; }
        public List<FieldDetailsDto> Fields { get; }

        public GetTagRequirementInfo(int id, List<FieldDetailsDto> fields)
        {
            Id = id;
            Fields = fields;
        }
    }
}
