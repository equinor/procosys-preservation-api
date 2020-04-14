using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagFunctionQuery : IRequest<Result<List<ProcosysTagDto>>>, IProjectRequest
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
