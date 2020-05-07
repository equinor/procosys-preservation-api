namespace Equinor.Procosys.Preservation.Domain
{
    public interface IBlobPathProvider
    {
        string CreatePathForAttachment(Attachment attachment);
    }
}
