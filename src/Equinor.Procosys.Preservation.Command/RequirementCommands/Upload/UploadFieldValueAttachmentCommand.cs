using System.IO;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementCommands.Upload
{
    public class UploadFieldValueAttachmentCommand : UploadAttachmentCommand, IRequest<Result<int>>, ITagCommandRequest
    {
        public UploadFieldValueAttachmentCommand(int tagId, int requirementId, int fieldId, string fileName, Stream content)
            : base(content)
        {
            TagId = tagId;
            RequirementId = requirementId;
            FileName = fileName;
            FieldId = fieldId;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public int FieldId { get; }
        public string FileName { get; }
    }
}
