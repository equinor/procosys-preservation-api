using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class TagDetailsDto
    {
        public int Id { get; set; }
        public string TagNo { get; set; }
        public string Description { get; set; }
        public PreservationStatus Status { get; set; }
        public string JourneyTitle { get; set; }
        public string Mode { get; set; }
        public string ResponsibleName { get; set; }
        public string CommPkgNo { get; set; }
        public string McPkgNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string AreaCode { get; set; }
    }
}
