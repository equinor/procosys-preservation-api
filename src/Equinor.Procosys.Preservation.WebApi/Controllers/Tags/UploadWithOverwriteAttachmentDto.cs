namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class UploadWithOverwriteAttachmentDto : UploadAttachmentDto
    {
        public bool OverwriteIfExists { get; set; }
    }
}
