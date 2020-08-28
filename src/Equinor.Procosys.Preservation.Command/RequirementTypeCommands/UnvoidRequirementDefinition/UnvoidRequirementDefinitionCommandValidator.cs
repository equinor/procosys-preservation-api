using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition
{
    public class UnvoidRequirementDefinitionCommandValidator : AbstractValidator<UnvoidRequirementDefinitionCommand>
    {
        public UnvoidRequirementDefinitionCommandValidator(
            IRequirementTypeValidator requirementTypeValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator,
            IRowVersionValidator rowVersionValidator
        )
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type doesn't exist! Requirement type={command.RequirementTypeId}")
                .MustAsync((command, token) => BeAnExistingRequirementDefinitionInTypeAsync(command.RequirementTypeId, command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition doesn't exist within given requirement type! Requirement definition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => BeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is not voided! Requirement definition={command.RequirementDefinitionId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(requirementTypeId, token); 
            async Task<bool> BeAnExistingRequirementDefinitionInTypeAsync(int requirementTypeId, int requirementDefinitionId, CancellationToken token)
                => await requirementTypeValidator.RequirementDefinitionExistsAsync(requirementTypeId, requirementDefinitionId, token);
            async Task<bool> BeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
