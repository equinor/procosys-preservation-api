using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementDefinition
{
    public class VoidRequirementDefinitionCommand : IRequest<Result<string>>
    {
        public VoidRequirementDefinitionCommand(int requirementTypeId, int requirementDefinitionId, string rowVersion, Guid currentUserOid)
        {
            RequirementTypeId = requirementTypeId;
            RequirementDefinitionId = requirementDefinitionId;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }

        public int RequirementTypeId { get; }
        public int RequirementDefinitionId { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
