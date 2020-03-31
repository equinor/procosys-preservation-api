using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagFunctionDetails
{
    public class GetTagFunctionDetailsQuery : IRequest<Result<TagFunctionDetailsDto>>
    {
        public GetTagFunctionDetailsQuery(string plant, string tagFunctionCode, string registerCode)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            TagFunctionCode = tagFunctionCode;
            RegisterCode = registerCode;
        }

        public string Plant { get; }
        public string TagFunctionCode { get; }
        public string RegisterCode { get; }
    }
}
