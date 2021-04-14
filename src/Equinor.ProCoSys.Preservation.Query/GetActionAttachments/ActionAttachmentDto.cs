namespace Equinor.ProCoSys.Preservation.Query.GetActionAttachments
{
    public class ActionAttachmentDto
    {
        public ActionAttachmentDto(int id, string fileName, string rowVersion)
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
