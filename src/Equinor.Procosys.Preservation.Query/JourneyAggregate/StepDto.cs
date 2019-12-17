namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class StepDto
    {
        public StepDto(int id, int modeId, int responsibleId)
        {
            Id = id;
            ModeId = modeId;
            ResponsibleId = responsibleId;
        }

        public int Id { get; }
        public int ModeId { get; }
        public int ResponsibleId { get; }
    }
}
