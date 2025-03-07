namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class UploadAttachmentWithOverwriteOptionDto : UploadAttachmentForceOverwriteDto
    {
        public bool OverwriteIfExists { get; set; }
    }
}
