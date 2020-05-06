namespace Equinor.Procosys.Preservation.Domain
{
    public interface IBlobPathProvider
    {
        string CreatePathForAttachment(string folderName, Attachment attachment);
    }
}
