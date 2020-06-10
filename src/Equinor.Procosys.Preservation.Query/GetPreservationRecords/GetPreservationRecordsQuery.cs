using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecords
{
    public class GetPreservationRecordsQuery : IRequest<Result<List<PreservationRecordDto>>>, ITagQueryRequest
    {
        public GetPreservationRecordsQuery(int tagId, int requirementId)
        {
            TagId = tagId;
            RequirementId = requirementId;
        }

        public int TagId { get; }
        public int RequirementId { get; }
    }
}
