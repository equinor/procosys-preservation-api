namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class ModeDto
    {
        public ModeDto(int id, string title, bool isVoided, bool forSupplier, bool inUse, string rowVersion)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            ForSupplier = forSupplier;
            InUse = inUse;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public bool ForSupplier { get; }
        public bool InUse { get; set; }
        public string RowVersion { get; }
    }
}
