using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecords
{
    public class GetPreservationRecordsQuery : IRequest<Result<List<PreservationRecordDto>>>, ITagQueryRequest
    {
        public GetPreservationRecordsQuery(int tagId, int tagRequirementId)
        {
            TagId = tagId;
            TagRequirementId = tagRequirementId;
        }

        public int TagId { get; }
        public int TagRequirementId { get; }
    }
}
