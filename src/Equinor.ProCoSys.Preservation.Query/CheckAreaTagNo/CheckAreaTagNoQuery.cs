using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.CheckAreaTagNo
{
    public class CheckAreaTagNoQuery : AbstractAreaTag, IRequest<Result<AreaTagDto>>, IProjectRequest
    {
        public CheckAreaTagNoQuery(string projectName, TagType tagType, string disciplineCode, string areaCode, string purchaseOrderCalloffCode, string tagNoSuffix)
        {
            ProjectName = projectName;
            TagType = tagType;
            DisciplineCode = disciplineCode;
            AreaCode = areaCode;
            PurchaseOrderCalloffCode = purchaseOrderCalloffCode;
            TagNoSuffix = tagNoSuffix;
        }

        public string ProjectName { get; }
        public override TagType TagType { get; }
        public override string DisciplineCode { get; }
        public override string AreaCode { get; }
        public override string PurchaseOrderCalloffCode { get; }
        public override string TagNoSuffix { get; }
    }
}
