using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public class GetTagRequirementInfo
    {
        public int Id { get; }
        public DateTime NextDueTimeUtc { get; }
        public List<FieldDetailsDto> Fields { get; }

        public GetTagRequirementInfo(int id, DateTime nextDueTimeUtc, List<FieldDetailsDto> fields)
        {
            Id = id;
            NextDueTimeUtc = nextDueTimeUtc;
            Fields = fields;
        }
    }
}
