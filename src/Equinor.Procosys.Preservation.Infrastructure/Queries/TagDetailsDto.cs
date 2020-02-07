using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class TagDetailsDto
    {
        public int Id { get; private set; }
        public string TagNo { get; private set; }
        public string Description { get; private set; }
        public PreservationStatus Status { get; private set; }
        public string JourneyName { get; private set; }
        public string ModeName { get; private set; }
        public string ResponsibleName { get; private set; }
        public string CommPkgNo { get; private set; }
        public string McPkgNo { get; private set; }
        public string PoNo { get; private set; }
        public string Area { get; private set; }
        public DateTime NextDueDate { get; private set; }
        public string NextDueDateString { get; private set; }
    }
}
