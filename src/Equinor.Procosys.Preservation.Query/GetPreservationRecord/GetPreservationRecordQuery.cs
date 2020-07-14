using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecord
{
    public class GetPreservationRecordQuery : IRequest<Result<PreservationRecordDto>>, ITagQueryRequest
    {
        public GetPreservationRecordQuery(int tagId, int requirementId, Guid preservationRecordGuid)
        {
            TagId = tagId;
            RequirementId = requirementId;
            PreservationRecordGuid = preservationRecordGuid;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public Guid PreservationRecordGuid { get; }
    }
}
