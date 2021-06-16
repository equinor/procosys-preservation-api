using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition
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
                .MustAsync(BeAnExistingRequirementDefinitionAsync)
                .WithMessage(_ => "Requirement type and/or requirement definition doesn't exist!")
                .MustAsync((command, token) => BeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is not voided! Requirement definition={command.RequirementDefinitionId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingRequirementDefinitionAsync(UnvoidRequirementDefinitionCommand command, CancellationToken token)
                => await requirementTypeValidator.RequirementDefinitionExistsAsync(command.RequirementTypeId, command.RequirementDefinitionId, token);
            async Task<bool> BeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
