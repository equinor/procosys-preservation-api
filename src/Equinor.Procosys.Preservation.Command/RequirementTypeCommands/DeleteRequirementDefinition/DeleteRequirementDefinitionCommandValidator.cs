using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.DeleteRequirementDefinition
{
    public class DeleteRequirementDefinitionCommandValidator : AbstractValidator<DeleteRequirementDefinitionCommand>
    {
        public DeleteRequirementDefinitionCommandValidator(
            IRequirementTypeValidator requirementTypeValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type doesn't exist! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => BeAnExistingRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition doesn't exist! RequirementDefinition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => BeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is not voided! RequirementDefinition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotHaveAnyFieldsAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition has fields! RequirementDefinition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotHaveAnyTagRequirementsAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Tag requirement with this requirement definition exists! RequirementDefinition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotHaveAnyTagFunctionRequirementsAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Tag function requirement with this requirement definition exists! RequirementDefinition={command.RequirementDefinitionId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid RowVersion! RowVersion={command.RowVersion}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(requirementTypeId, token);
            async Task<bool> BeAnExistingRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => await requirementDefinitionValidator.ExistsAsync(requirementDefinitionId, token);
            async Task<bool> BeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            async Task<bool> NotHaveAnyFieldsAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.FieldsExistAsync(requirementDefinitionId, token);
            async Task<bool> NotHaveAnyTagRequirementsAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.TagRequirementsExistAsync(requirementDefinitionId, token);
            async Task<bool> NotHaveAnyTagFunctionRequirementsAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.TagFunctionRequirementsExistAsync(requirementDefinitionId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
