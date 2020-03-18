using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagFunctionDetails
{
    public class GetTagFunctionQuery : IRequest<Result<TagFunctionDetailsDto>>
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
