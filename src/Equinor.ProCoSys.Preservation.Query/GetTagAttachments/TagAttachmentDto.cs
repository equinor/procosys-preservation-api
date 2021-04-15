namespace Equinor.ProCoSys.Preservation.Query.GetTagAttachments
{
    public class TagAttachmentDto
    {
        public TagAttachmentDto(int id, string fileName, string rowVersion)
        {
            Id = id;
            FileName = fileName;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string FileName { get; }
        public string RowVersion { get; }
    }
}
