using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagFunctionQuery : IRequest<Result<List<ProcosysTagDto>>>
    {
        public SearchTagsByTagFunctionQuery(string plant, string projectName, string tagFunctionCode, string registerCode)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            ProjectName = projectName;
            TagFunctionCode = tagFunctionCode;
            RegisterCode = registerCode;
        }

        public string Plant { get; }
        public string ProjectName { get; }
        public string TagFunctionCode { get; }
        public string RegisterCode { get; }
    }
}
