namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentDto
    {
        public string Title { get; set; }
        public string FileName { get; set; }
        public bool OverwriteIfExists { get; set; }
    }
}
