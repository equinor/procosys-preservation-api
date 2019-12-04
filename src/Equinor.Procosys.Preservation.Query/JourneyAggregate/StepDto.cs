namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class StepDto
    {
        public StepDto(int id, int modeId, int responsibleId, string schema)
        {
            Id = id;
            ModeId = modeId;
            ResponsibleId = responsibleId;
            Schema = schema;
        }

        public int Id { get; }
        public int ModeId { get; }
        public int ResponsibleId { get; }
        public string Schema { get; }
    }
}
