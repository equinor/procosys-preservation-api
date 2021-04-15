using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Query.ModeAggregate;
using Equinor.ProCoSys.Preservation.Query.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Query.GetJourneyById
{
    public class StepDetailsDto
    {
        public StepDetailsDto(int id,
            string title,
            bool isInUse,
            bool isVoided,
            ModeDto mode,
            ResponsibleDto responsible,
            AutoTransferMethod autoTransferMethod,
            string rowVersion)
        {
            Id = id;
            Title = title;
            IsInUse = isInUse;
            IsVoided = isVoided;
            Mode = mode;
            Responsible = responsible;
            AutoTransferMethod = autoTransferMethod;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsInUse { get; }
        public bool IsVoided { get; }
        public ModeDto Mode { get; }
        public ResponsibleDto Responsible { get; }
        public AutoTransferMethod AutoTransferMethod { get; }
        public string RowVersion { get; }
    }
}
