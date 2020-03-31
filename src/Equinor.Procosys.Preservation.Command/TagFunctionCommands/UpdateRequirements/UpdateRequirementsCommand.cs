using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagFunctionCommands.UpdateRequirements
{
    public class UpdateRequirementsCommand : IRequest<Result<Unit>>
    {
        public UpdateRequirementsCommand(
            string plant,
            string tagFunctionCode,
            string registerCode,
            IEnumerable<Requirement> requirements)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            TagFunctionCode = tagFunctionCode;
            RegisterCode = registerCode;
            Requirements = requirements ?? new List<Requirement>();
        }

        public string Plant { get; }
        public string TagFunctionCode { get; }
        public string RegisterCode { get; }
        public IEnumerable<Requirement> Requirements { get; }
    }
}
