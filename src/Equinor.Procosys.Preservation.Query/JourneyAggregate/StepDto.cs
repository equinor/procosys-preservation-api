using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class StepDto
    {
        public StepDto(int id, string title, bool isVoided, string rowVersion, ModeDto mode, ResponsibleDto responsible)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            RowVersion = rowVersion;
            Mode = mode;
            Responsible = responsible;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public string RowVersion { get; }
        public ModeDto Mode { get; }
        public ResponsibleDto Responsible { get; }
    }
}
