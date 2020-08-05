using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.DeleteRequirementType
{
    public class DeleteRequirementTypeCommandValidator : AbstractValidator<DeleteRequirementTypeCommand>
    {
        public DeleteRequirementTypeCommandValidator(
            IRequirementTypeValidator requirementTypeValidator,
            IRowVersionValidator rowVersionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type doesn't exist! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => NotHaveAnyRequirementDefinitions(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type has requirement definitions! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => BeAVoidedRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type is not voided! RequirementType={command.RequirementTypeId}")
                .Must(command => HaveAValidRowVersion(command.RowVersion))
                .WithMessage(command => $"Not a valid RowVersion! RowVersion={command.RowVersion}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(requirementTypeId, token);
            async Task<bool> NotHaveAnyRequirementDefinitions(int requirementTypeId, CancellationToken token)
                => !await requirementTypeValidator.AnyRequirementDefinitionExistsAsync(requirementTypeId, token);
            async Task<bool> BeAVoidedRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.IsVoidedAsync(requirementTypeId, token);
            bool HaveAValidRowVersion(string rowVersion)
                => rowVersionValidator.IsValid(rowVersion);
        }
    }
}
