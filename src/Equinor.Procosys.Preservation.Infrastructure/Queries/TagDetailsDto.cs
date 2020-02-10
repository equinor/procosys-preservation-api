using System;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class TagDetailsDto
    {
        public int Id { get; set; }
        public string TagNo { get; set; }
        public string Description { get; set; }
        public PreservationStatus Status { get; set; }
        public string JourneyName { get; set; }
        public string Mode { get; set; }
        public string Responsible { get; set; }
        public string CommPkgNo { get; set; }
        public string McPkgNo { get; set; }
        public string PoNo { get; set; }
        public string Area { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string NextDueDateString => NextDueDate?.FormatAsYearAndWeekString() ?? null;
    }
}
