using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagFunctionQuery : IRequest<Result<List<ProcosysTagDto>>>
    {
        public SearchTagsByTagFunctionQuery(string projectName, string tagFunctionCode, string registerCode)
        {
            ProjectName = projectName;
            TagFunctionCode = tagFunctionCode;
            RegisterCode = registerCode;
        }

        public string ProjectName { get; }
        public string TagFunctionCode { get; }
        public string RegisterCode { get; }
    }
}
