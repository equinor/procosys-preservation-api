namespace Equinor.Procosys.Preservation.Domain
{
    public interface IBlobPathProvider
    {
        string CreatePathForAttachment<T>(Attachment attachment) where T : class;
    }
}
