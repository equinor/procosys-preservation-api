namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class RequirementDto
    {
        // todo Unit test
        public RequirementDto(int id, bool isVoided, int interval)
        {
            Id = id;
            IsVoided = isVoided;
            Interval = interval;
        }

        public int Id { get; set; }
        public bool IsVoided { get; }
        public int Interval { get; }
    }
}
