using Equinor.Procosys.Preservation.Query.GetMode;
using Equinor.Procosys.Preservation.Query.GetResponsibles;

namespace Equinor.Procosys.Preservation.Query.GetJourney
{
    public class StepDto
    {
        public StepDto(int id, string title, bool isVoided, ModeDto mode, ResponsibleDto responsible, string rowVersion)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            Mode = mode;
            Responsible = responsible;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public ModeDto Mode { get; }
        public ResponsibleDto Responsible { get; }
        public string RowVersion { get; }
    }
}
