using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class RequirementDto
    {
        public RequirementDto(int id, DateTime? nextDueTimeUtc, bool readyToBePreserved, List<FieldDto> fields)
        {
            Id = id;
            NextDueTimeUtc = nextDueTimeUtc;
            ReadyToBePreserved = readyToBePreserved;
            Fields = fields;
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
        }

        public int Id { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }
        public bool ReadyToBePreserved { get; }
        public List<FieldDto> Fields { get; }
    }
}
