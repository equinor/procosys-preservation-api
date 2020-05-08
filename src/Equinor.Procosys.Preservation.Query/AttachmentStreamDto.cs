using System.IO;

namespace Equinor.Procosys.Preservation.Query
{
    public class AttachmentStreamDto
    {
        public AttachmentStreamDto(int id, string fileName, Stream openStream)
        {
            Id = id;
            FileName = fileName;
            Content = openStream;
        }

        public int Id { get; }
        public string FileName { get; }
        public Stream Content { get; }
    }
}
