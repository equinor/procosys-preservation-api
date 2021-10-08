using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant
{
    public class TagDto
    {
        public TagDto(
            string plantId,
            string plantTitle,
            string projectName,
            string projectDescription,
            bool projectIsClosed,
            int id,
            ActionStatus? actionStatus,
            string areaCode,
            string areaDescription,
            string calloff,
            string commPkgNo,
            string description,
            string disciplineCode,
            string disciplineDescription,
            string mcPkgNo,
            string purchaseOrderNo,
            bool readyToBePreserved,
            IEnumerable<RequirementDto> requirementDtos,
            PreservationStatus status,
            int stepId,
            string tagFunctionCode,
            string tagNo,
            TagType tagType)
        {
            PlantId = plantId;
            PlantTitle = plantTitle;
            ProjectName = projectName;
            ProjectDescription = projectDescription;
            ProjectIsClosed = projectIsClosed;
            Id = id;
            ActionStatus = actionStatus;
            AreaCode = areaCode;
            AreaDescription = areaDescription;
            Calloff = calloff;
            CommPkgNo = commPkgNo;
            Description = description;
            DisciplineCode = disciplineCode;
            DisciplineDescription = disciplineDescription;
            McPkgNo = mcPkgNo;
            PurchaseOrderNo = purchaseOrderNo;
            ReadyToBePreserved = readyToBePreserved;
            Requirements = requirementDtos;
            Status = status;
            StepId = stepId;
            TagFunctionCode = tagFunctionCode;
            TagNo = tagNo;
            TagType = tagType;
        }

        public string PlantId { get; }
        public string PlantTitle { get; private set; }
        public string ProjectName { get; }
        public string ProjectDescription { get; }
        public bool ProjectIsClosed { get; }
        public int Id { get; }
        public ActionStatus? ActionStatus { get; }
        public string AreaCode { get; }
        public string AreaDescription { get; }
        public string Calloff { get; }
        public string CommPkgNo { get; }
        public string Description { get; }
        public string DisciplineCode { get; }
        public string DisciplineDescription { get; }
        public string McPkgNo { get; }
        public string Mode { get; private set; }
        public string NextMode { get; private set; }
        public string NextResponsibleCode { get; private set; }
        public string NextResponsibleDescription { get; private set; }
        public string PurchaseOrderNo { get; }
        public bool ReadyToBePreserved { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
        public string ResponsibleCode { get; private set; }
        public string ResponsibleDescription { get; private set; }
        public PreservationStatus Status { get; }
        public string TagFunctionCode { get; }
        public string TagNo { get; }
        public TagType TagType { get; }

        // don't expose StepId
        internal int StepId { get; }

        public void SetJouyrneyData(
            string mode,
            string resposibleCode,
            string resposibleDescription,
            string nextMode,
            string nextResposibleCode,
            string nextResposibleDescription)
        {
            Mode = mode;
            ResponsibleCode = resposibleCode;
            ResponsibleDescription = resposibleDescription;

            if (TagType.FollowsAJourney())
            {
                NextMode = nextMode;
                NextResponsibleCode = nextResposibleCode;
                NextResponsibleDescription = nextResposibleDescription;
            }
        }
    }
}
