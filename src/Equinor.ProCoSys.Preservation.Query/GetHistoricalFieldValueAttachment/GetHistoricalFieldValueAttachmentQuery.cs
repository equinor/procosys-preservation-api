using System;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetHistoricalFieldValueAttachment
{
    public class GetHistoricalFieldValueAttachmentQuery : IRequest<Result<Uri>>, ITagQueryRequest
    {
        public GetHistoricalFieldValueAttachmentQuery(
            int tagId,
            int tagRequirementId,
            Guid preservationRecordGuid)
        {
            TagId = tagId;
            TagRequirementId = tagRequirementId;
            PreservationRecordGuid = preservationRecordGuid;
        }

        public int TagId { get; }
        public int TagRequirementId { get; }
        public Guid PreservationRecordGuid { get; }
    }
}
