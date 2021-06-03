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
            bool isProjectClosed,
            int id,
            ActionStatus? actionStatus,
            string areaCode,
            string areaDescription,
            string callOff,
            string commPkgNo,
            string description,
            string disciplineCode,
            string disciplineDescription,
            bool isVoided,
            string mcPkgNo,
            string mode,
            string nextMode,
            string nextResponsibleCode,
            string nextResponsibleDescription,
            string purchaseOrderNo,
            bool readyToBePreserved,
            IEnumerable<RequirementDto> requirements,
            string responsibleCode,
            string responsibleDescription,
            PreservationStatus status,
            string tagFunctionCode,
            string tagNo,
            TagType tagType)
        {
            PlantId = plantId;
            PlantTitle = plantTitle;
            ProjectName = projectName;
            ProjectDescription = projectDescription;
            IsProjectClosed = isProjectClosed;
            Id = id;
            ActionStatus = actionStatus;
            AreaCode = areaCode;
            AreaDescription = areaDescription;
            CallOff = callOff;
            CommPkgNo = commPkgNo;
            Description = description;
            DisciplineCode = disciplineCode;
            DisciplineDescription = disciplineDescription;
            IsVoided = isVoided;
            McPkgNo = mcPkgNo;
            Mode = mode;
            NextMode = nextMode;
            NextResponsibleCode = nextResponsibleCode;
            NextResponsibleDescription = nextResponsibleDescription;
            PurchaseOrderNo = purchaseOrderNo;
            ReadyToBePreserved = readyToBePreserved;
            Requirements = requirements;
            ResponsibleCode = responsibleCode;
            ResponsibleDescription = responsibleDescription;
            Status = status;
            TagFunctionCode = tagFunctionCode;
            TagNo = tagNo;
            TagType = tagType;
        }

        public string PlantId { get; }
        public string PlantTitle { get; }
        public string ProjectName { get; }
        public string ProjectDescription { get; }
        public bool IsProjectClosed { get; }
        public int Id { get; }
        public ActionStatus? ActionStatus { get; }
        public string AreaCode { get; }
        public string AreaDescription { get; }
        public string CallOff { get; }
        public string CommPkgNo { get; }
        public string Description { get; }
        public string DisciplineCode { get; }
        public string DisciplineDescription { get; }
        public bool IsVoided { get; }
        public string McPkgNo { get; }
        public string Mode { get; }
        public string NextMode { get; }
        public string NextResponsibleCode { get; }
        public string NextResponsibleDescription { get; }
        public string PurchaseOrderNo { get; }
        public bool ReadyToBePreserved { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
        public string ResponsibleCode { get; }
        public string ResponsibleDescription { get; }
        public PreservationStatus Status { get; }
        public string TagFunctionCode { get; }
        public string TagNo { get; }
        public TagType TagType { get; }
    }
}
