using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagFunctionDetails
{
    public class GetTagFunctionDetailsQuery : IRequest<Result<TagFunctionDetailsDto>>
    {
        public GetTagFunctionDetailsQuery(string tagFunctionCode, string registerCode)
        {
            TagFunctionCode = tagFunctionCode;
            RegisterCode = registerCode;
        }

        public string TagFunctionCode { get; }
        public string RegisterCode { get; }
    }
}
