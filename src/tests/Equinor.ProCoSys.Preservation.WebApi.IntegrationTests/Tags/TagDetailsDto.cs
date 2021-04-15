using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class TagDetailsDto
    {
        public int Id { get; set; }
        public string TagNo { get; set; }
        public bool IsInUse { get; set; }
        public bool IsVoided { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public JourneyDetailsDto Journey { get; set; }
        public StepDetailsDto Step { get; set; }
        public ModeDetailsDto Mode { get; set; }
        public ResponsibleDetailsDto Responsible { get; set; }
        public string CommPkgNo { get; set; }
        public string McPkgNo { get; set; }
        public string CalloffNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string AreaCode { get; set; }
        public string DisciplineCode { get; set; }
        public TagType TagType { get; set; }
        public bool ReadyToBePreserved { get; set; }
        public string Remark { get; set; }
        public string StorageArea { get; set; }
        public string RowVersion { get; set; }
    }
}
