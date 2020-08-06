using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Query.GetJourneyById
{
    public class StepDetailsDto
    {
        public StepDetailsDto(int id,
            string title,
            bool isVoided,
            ModeDto mode,
            ResponsibleDto responsible,
            bool transferOnRfccSign,
            bool transferOnRfocSign,
            string rowVersion)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            Mode = mode;
            Responsible = responsible;
            TransferOnRfccSign = transferOnRfccSign;
            TransferOnRfocSign = transferOnRfocSign;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public ModeDto Mode { get; }
        public ResponsibleDto Responsible { get; }
        public bool TransferOnRfccSign { get; }
        public bool TransferOnRfocSign { get; }
        public string RowVersion { get; }
    }
}
