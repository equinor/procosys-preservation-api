namespace Equinor.ProCoSys.Preservation.Query.ModeAggregate
{
    public class ModeDto
    {
        public ModeDto(int id, string title, bool isVoided, bool forSupplier, bool isInUse, string rowVersion)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            ForSupplier = forSupplier;
            IsInUse = isInUse;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public bool ForSupplier { get; }
        public bool IsInUse { get; set; }
        public string RowVersion { get; }
    }
}
