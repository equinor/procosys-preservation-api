﻿using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagFunctionCommands.UpdateRequirements
{
    public class UpdateRequirementsCommand : IRequest<Result<Unit>>
    {
        public UpdateRequirementsCommand(
            string tagFunctionCode,
            string registerCode,
            IEnumerable<RequirementForCommand> requirements,
            string rowVersion)
        {
            TagFunctionCode = tagFunctionCode;
            RegisterCode = registerCode;
            Requirements = requirements ?? new List<RequirementForCommand>();
            RowVersion = rowVersion;
        }

        public string TagFunctionCode { get; }
        public string RegisterCode { get; }
        public IEnumerable<RequirementForCommand> Requirements { get; }
        public string RowVersion { get; }
    }
}
