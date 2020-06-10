using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Query.GetPreservationRecords;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class TagRequirementDto
    {
        public TagRequirementDto(
            int id, 
            string rowVersion)
        {
            Id = id;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string RowVersion { get; }
    }
}
