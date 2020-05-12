namespace Equinor.Procosys.Preservation.Query.GetTagAttachments
{
    public class TagAttachmentDto
    {
        public TagAttachmentDto(int id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }

        public int Id { get; }
        public string FileName { get; }
    }
}
