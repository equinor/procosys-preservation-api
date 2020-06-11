using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecord
{
    public class GetPreservationRecordQuery : IRequest<Result<PreservationRecordDto>>, ITagQueryRequest
    {
        public GetPreservationRecordQuery(int tagId, int requirementId, int preservationRecordId)
        {
            TagId = tagId;
            RequirementId = requirementId;
            PreservationRecordId = preservationRecordId;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public int PreservationRecordId { get; }
    }
}
