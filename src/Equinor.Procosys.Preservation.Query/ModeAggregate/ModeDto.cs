namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class ModeDto
    {
        public ModeDto(int id, string title, bool isVoided, bool forSupplier, string rowVersion)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            ForSupplier = forSupplier;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public bool ForSupplier { get; }
        public string RowVersion { get; }
    }
}
