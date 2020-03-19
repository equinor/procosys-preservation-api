using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagFunctionsQuery : IRequest<Result<List<ProcosysTagDto>>>
    {
        public SearchTagsByTagFunctionsQuery(string projectName, IEnumerable<string> tagFunctionCodeRegisterCodePairs)
        {
            ProjectName = projectName;
            TagFunctionCodeRegisterCodePairs = tagFunctionCodeRegisterCodePairs;
        }

        public string ProjectName { get; }
        public IEnumerable<string> TagFunctionCodeRegisterCodePairs { get; }
    }
}
