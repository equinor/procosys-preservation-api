namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class StepDetailsDto
    {
        public StepDetailsDto(int id, string title, ModeDetailsDto mode, ResponsibleDetailsDto responsible)
        {
            Id = id;
            Title = title;
            Mode = mode;
            Responsible = responsible;
        }

        public int Id { get; }
        public string Title { get; }
        public ModeDetailsDto Mode { get; }
        public ResponsibleDetailsDto Responsible { get; }
    }
}
