namespace Equinor.Procosys.Preservation.Query.GetTagAttachments
{
    public class TagAttachmentDto
    {
        public TagAttachmentDto(int id, string title, string fileName)
        {
            Id = id;
            Title = title;
            FileName = fileName;
        }

        public int Id { get; }
        public string Title { get; }
        public string FileName { get; }
    }
}
