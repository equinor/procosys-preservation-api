using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.VoidRequirementDefinition
{
    public class VoidRequirementDefinitionCommandValidator : AbstractValidator<VoidRequirementDefinitionCommand>
    {
        public VoidRequirementDefinitionCommandValidator(
            IRequirementTypeValidator requirementTypeValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator,
            IRowVersionValidator rowVersionValidator
        )
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync(BeAnExistingRequirementDefinitionAsync)
                .WithMessage(_ => "Requirement type and/or requirement definition doesn't exist!")
                .MustAsync((command, token) => NotBeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is already voided! Requirement definition={command.RequirementDefinitionId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid row version! Row version={command.RowVersion}");

            async Task<bool> BeAnExistingRequirementDefinitionAsync(VoidRequirementDefinitionCommand command, CancellationToken token)
                => await requirementTypeValidator.RequirementDefinitionExistsAsync(command.RequirementTypeId, command.RequirementDefinitionId, token);
            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
