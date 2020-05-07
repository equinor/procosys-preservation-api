namespace Equinor.Procosys.Preservation.Domain
{
    public interface IBlobPathProvider
    {
        string CreateFullBlobPathForAttachment(Attachment attachment);
    }
}
