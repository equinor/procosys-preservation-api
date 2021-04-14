using System;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetPreservationRecord
{
    public class GetPreservationRecordQuery : IRequest<Result<PreservationRecordDto>>, ITagQueryRequest
    {
        public GetPreservationRecordQuery(int tagId, int tagRequirementId, Guid preservationRecordGuid)
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
