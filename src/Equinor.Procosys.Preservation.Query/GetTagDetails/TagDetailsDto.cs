using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class TagDetailsDto
    {
        public TagDetailsDto(
            int id,
            string tagNo,
            string description,
            PreservationStatus status,
            string journeyName,
            string modeName,
            string responsibleName,
            string commPkgNo,
            string mcPkgNo,
            string poNo,
            string area,
            DateTime nextDueDate,
            string nextDueDateString)
        {
            Id = id;
            TagNo = tagNo;
            Description = description;
            Status = status;
            JourneyName = journeyName;
            ModeName = modeName;
            ResponsibleName = responsibleName;
            CommPkgNo = commPkgNo;
            McPkgNo = mcPkgNo;
            PoNo = poNo;
            Area = area;
            NextDueDate = nextDueDate;
            NextDueDateString = nextDueDateString;
        }

        public int Id { get; }
        public string TagNo { get; }
        public string Description { get; }
        public PreservationStatus Status { get; }
        public string JourneyName { get; }
        public string ModeName { get; }
        public string ResponsibleName { get; }
        public string CommPkgNo { get; }
        public string McPkgNo { get; }
        public string PoNo { get; }
        public string Area { get; }
        public DateTime NextDueDate { get; }
        public string NextDueDateString { get; }
    }
}
