using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.DeleteRequirementDefinition
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
                .MustAsync(BeAnExistingRequirementDefinitionAsync)
                .WithMessage(command => "Requirement type and/or requirement definition doesn't exist!")
                .MustAsync((command, token) => BeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is not voided! Requirement definition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotHaveAnyFieldsAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition has fields! Requirement definition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotHaveAnyTagRequirementsAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Tag requirement with this requirement definition exists! Requirement definition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotHaveAnyTagFunctionRequirementsAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Tag function requirement with this requirement definition exists! Requirement definition={command.RequirementDefinitionId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingRequirementDefinitionAsync(DeleteRequirementDefinitionCommand command, CancellationToken token)
                => await requirementTypeValidator.RequirementDefinitionExistsAsync(command.RequirementTypeId, command.RequirementDefinitionId, token);
            async Task<bool> BeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            async Task<bool> NotHaveAnyFieldsAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.HasAnyFieldsAsync(requirementDefinitionId, token);
            async Task<bool> NotHaveAnyTagRequirementsAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.TagRequirementsExistAsync(requirementDefinitionId, token);
            async Task<bool> NotHaveAnyTagFunctionRequirementsAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.TagFunctionRequirementsExistAsync(requirementDefinitionId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
