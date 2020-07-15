using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetHistoricalFieldValueAttachment
{
    public class GetHistoricalFieldValueAttachmentQuery : IRequest<Result<Uri>>, ITagQueryRequest
    {
        public GetHistoricalFieldValueAttachmentQuery(
            int tagId,
            int tagTagRequirementId,
            Guid preservationRecordGuid)
        {
            TagId = tagId;
            TagRequirementId = tagTagRequirementId;
            PreservationRecordGuid = preservationRecordGuid;
        }

        public int TagId { get; }
        public int TagRequirementId { get; }
        public Guid PreservationRecordGuid { get; }
    }
}
