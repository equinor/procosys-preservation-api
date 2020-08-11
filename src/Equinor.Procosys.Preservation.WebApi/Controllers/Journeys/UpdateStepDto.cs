using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Journeys
{
    public class UpdateStepDto
    {
        public int ModeId { get; set; }
        public string ResponsibleCode { get; set; }
        public string Title { get; set; }
        public AutoTransferMethod AutoTransferMethod { get; set; }
        public string RowVersion { get; set; }
    }
}
