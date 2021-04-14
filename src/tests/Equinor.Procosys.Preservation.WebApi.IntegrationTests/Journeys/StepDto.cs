using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys
{
    public class StepDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsVoided { get; set; }
        public AutoTransferMethod AutoTransferMethod { get; set; }
        public ModeDto Mode { get; set; }
        public string RowVersion { get; set; }
    }
}
