namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class UploadAttachmentWithOverwriteOptionDto : UploadAttachmentForceOverwriteDto
    {
        public bool OverwriteIfExists { get; set; }
    }
}
