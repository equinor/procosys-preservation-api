using System;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetFieldValueAttachment
{
    public class GetFieldValueAttachmentQuery : IRequest<Result<Uri>>, ITagQueryRequest
    {
        public GetFieldValueAttachmentQuery(int tagId, int requirementId, int fieldId)
        {
            TagId = tagId;
            RequirementId = requirementId;
            FieldId = fieldId;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public int FieldId { get; }
    }
}
