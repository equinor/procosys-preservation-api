using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementDefinition
{
    public class VoidRequirementDefinitionCommandValidator : AbstractValidator<VoidRequirementDefinitionCommand>
    {
        public VoidRequirementDefinitionCommandValidator(
            IRequirementTypeValidator requirementTypeValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator,
            IRowVersionValidator rowVersionValidator
        )
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type does not exist! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => BeAnExistingRequirementDefinitionAsync(command.RequirementTypeId, command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition does not exist! RequirementDefinition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotBeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is already voided! RequirementDefinition={command.RequirementDefinitionId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid RowVersion! RowVersion={command.RowVersion}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(requirementTypeId, token); 
            async Task<bool> BeAnExistingRequirementDefinitionAsync(int requirementTypeId, int requirementDefinitionId, CancellationToken token)
                => await requirementTypeValidator.RequirementDefinitionExistsAsync(requirementTypeId, requirementDefinitionId, token);
            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
