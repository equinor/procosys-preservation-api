using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Query.GetAllJourneys
{
    public class StepDto
    {
        public StepDto(int id, string title, bool isVoided, ModeDto mode, ResponsibleDto responsible, AutoTransferMethod autoTransferMethod, string rowVersion)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            Mode = mode;
            Responsible = responsible;
            AutoTransferMethod = autoTransferMethod;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public AutoTransferMethod AutoTransferMethod { get; }
        public ModeDto Mode { get; }
        public ResponsibleDto Responsible { get; }
        public string RowVersion { get; }
    }
}
