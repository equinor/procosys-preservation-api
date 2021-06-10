using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys
{
    public class StepDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsInUse { get; set; }
        public bool IsVoided { get; set; }
        public ModeDto Mode { get; set; }
        public ResponsibleDto Responsible { get; set; }
        public AutoTransferMethod AutoTransferMethod { get; set; }
        public string RowVersion { get; set; }
    }
}
