namespace Equinor.Procosys.Preservation.Domain
{
    public interface IBlobPathProvider
    {
        string CreateFullPathForAttachment(Attachment attachment);
    }
}
