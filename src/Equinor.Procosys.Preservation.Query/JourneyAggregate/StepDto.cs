using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class StepDto
    {
        public StepDto(int id, bool isVoided, ModeDto mode, ResponsibleDto responsible)
        {
            Id = id;
            IsVoided = isVoided;
            Mode = mode;
            Responsible = responsible;
        }

        public int Id { get; }
        public bool IsVoided { get; }
        public ModeDto Mode { get; }
        public ResponsibleDto Responsible { get; }
    }
}
