using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    public class TagDto
    {
        public string PlantId { get; set; }
        public string PlantTitle { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public bool ProjectIsClosed { get; set; }
        public int Id { get; set; }
        public ActionStatus? ActionStatus { get; set; }
        public string AreaCode { get; set; }
        public string AreaDescription { get; set; }
        public string Calloff { get; set; }
        public string CommPkgNo { get; set; }
        public string Description { get; set; }
        public string DisciplineCode { get; set; }
        public string DisciplineDescription { get; set; }
        public bool IsVoided { get; set; }
        public string McPkgNo { get; set; }
        public string Mode { get; set; }
        public string NextMode { get; set; }
        public string NextResponsibleCode { get; set; }
        public string NextResponsibleDescription { get; set; }
        public string PurchaseOrderNo { get; set; }
        public bool ReadyToBePreserved { get; set; }
        public IEnumerable<RequirementDto> Requirements { get; set; }
        public string ResponsibleCode { get; set; }
        public string ResponsibleDescription { get; set; }
        public PreservationStatus Status { get; set; }
        public string TagFunctionCode { get; set; }
        public string TagNo { get; set; }
        public TagType TagType { get; set; }

        // don't expose StepId
        internal int StepId { get; set; }

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
