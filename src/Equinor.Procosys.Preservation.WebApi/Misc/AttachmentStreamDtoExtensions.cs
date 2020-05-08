using HeyRed.Mime;
using System.IO;
using Equinor.Procosys.Preservation.Query;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public static class AttachmentStreamDtoExtensions
    {
        public static string GetMimeType(this AttachmentStreamDto dto)
            => dto?.FileName != null ? MimeTypesMap.GetMimeType(Path.GetExtension(dto.FileName)) : string.Empty;
    }
}
