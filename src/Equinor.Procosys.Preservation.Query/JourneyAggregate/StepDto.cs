using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class StepDto
    {
        public StepDto(int id, ModeDto mode, ResponsibleDto responsible)
        {
            Id = id;
            Mode = mode;
            Responsible = responsible;
        }

        public int Id { get; }
        public ModeDto Mode { get; }
        public ResponsibleDto Responsible { get; }
    }
}
