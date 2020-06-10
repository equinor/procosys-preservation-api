using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecords
{
    public class PreservationRecordDto
    {
        public PreservationRecordDto(
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
