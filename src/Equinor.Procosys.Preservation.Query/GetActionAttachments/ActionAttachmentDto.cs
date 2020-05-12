namespace Equinor.Procosys.Preservation.Query.GetActionAttachments
{
    public class ActionAttachmentDto
    {
        public ActionAttachmentDto(int id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }

        public int Id { get; }
        public string FileName { get; }
    }
}
