using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagFunctionAggregate
{
    public class GetTagFunctionQuery : IRequest<Result<TagFunctionDto>>
    {
        public GetTagFunctionQuery(string tagFunctionCode, string registerCode)
        {
            TagFunctionCode = tagFunctionCode;
            RegisterCode = registerCode;
        }

        public string TagFunctionCode { get; }
        public string RegisterCode { get; }
    }
}
