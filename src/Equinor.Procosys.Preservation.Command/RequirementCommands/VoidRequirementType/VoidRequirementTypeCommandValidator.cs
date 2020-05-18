using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementType;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.VoidRequirementType
{
    public class VoidRequirementTypeCommandValidator : AbstractValidator<VoidRequirementTypeCommand>
    {
        public VoidRequirementTypeCommandValidator(IRequirementTypeValidator requirementTypeValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type doesn't exist! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => NotBeAVoidedRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type is already voided! RequirementType={command.RequirementTypeId}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int modeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(modeId, token);
            async Task<bool> NotBeAVoidedRequirementTypeAsync(int modeId, CancellationToken token)
                => !await requirementTypeValidator.IsVoidedAsync(modeId, token);
        }
    }
}
