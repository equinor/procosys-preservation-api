using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Query.GetTagsQueries;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public class TagDto
    {
        public int Id { get; set; }
        public ActionStatus? ActionStatus { get; set; }
        public string AreaCode { get; set; }
        public string CalloffNo { get; set; }
        public string CommPkgNo { get; set; }
        public string Description { get; set; }
        public string DisciplineCode { get; set; }
        public bool IsNew { get; set; }
        public bool IsVoided { get; set; }
        public string McPkgNo { get; set; }
        public string Mode { get; set; }
        public string NextMode { get; set; }
        public string NextResponsibleCode { get; set; }
        public string PurchaseOrderNo { get; set; }
        public bool ReadyToBePreserved { get; set; }
        public bool ReadyToBeStarted { get; set; }
        public bool ReadyToBeTransferred { get; set; }
        public bool ReadyToBeCompleted { get; set; }
        public bool ReadyToBeRescheduled { get; set; }
        public bool ReadyToBeDuplicated { get; set; }
        public IEnumerable<RequirementDto> Requirements { get; set; }
        public string ResponsibleCode { get; set; }
        public string ResponsibleDescription { get; set; }
        public string Status { get; set; }
        public string StorageArea { get; set; }
        public string TagFunctionCode { get; set; }
        public string TagNo { get; set; }
        public TagType TagType { get; set; }
        public string RowVersion { get; set; }    }
}
