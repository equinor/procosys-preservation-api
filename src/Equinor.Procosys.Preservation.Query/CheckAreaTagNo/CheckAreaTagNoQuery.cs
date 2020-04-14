using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.CheckAreaTagNo
{
    public class CheckAreaTagNoQuery : AbstractAreaTag, IRequest<Result<AreaTagDto>>
    {
        public CheckAreaTagNoQuery(string projectName, TagType tagType, string disciplineCode, string areaCode, string tagNoSuffix)
        {
            ProjectName = projectName;
            TagType = tagType;
            DisciplineCode = disciplineCode;
            AreaCode = areaCode;
            TagNoSuffix = tagNoSuffix;
        }

        public string ProjectName { get; }
        public override TagType TagType { get; }
        public override string DisciplineCode { get; }
        public override string AreaCode { get; }
        public override string TagNoSuffix { get; }
    }
}
