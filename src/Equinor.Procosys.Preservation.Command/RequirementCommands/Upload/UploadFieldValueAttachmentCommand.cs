using System;
using System.IO;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.Upload
{
    public class UploadFieldValueAttachmentCommand : UploadAttachmentCommand, IRequest<Result<int>>, ITagCommandRequest
    {
        public UploadFieldValueAttachmentCommand(int tagId, int requirementId, int fieldId, string fileName, Stream content, Guid currentUserOid)
            : base(content)
        {
            TagId = tagId;
            RequirementId = requirementId;
            FileName = fileName;
            CurrentUserOid = currentUserOid;
            FieldId = fieldId;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public int FieldId { get; }
        public string FileName { get; }
        public Guid CurrentUserOid { get; }
    }
}
