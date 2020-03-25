using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagFunctionCommands.UpdateRequirements
{
    public class UpdateRequirementsCommand : IRequest<Result<Unit>>
    {
        public UpdateRequirementsCommand(
            string tagFunctionCode,
            string registerCode,
            IEnumerable<Requirement> requirements)
        {
            TagFunctionCode = tagFunctionCode;
            RegisterCode = registerCode;
            Requirements = requirements ?? new List<Requirement>();
        }

        public string TagFunctionCode { get; }
        public string RegisterCode { get; }
        public IEnumerable<Requirement> Requirements { get; }
    }
}
