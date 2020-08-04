using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementType
{
    public class UpdateRequirementTypeCommandValidator : AbstractValidator<UpdateRequirementTypeCommand>
    {
        public UpdateRequirementTypeCommandValidator(IRequirementTypeValidator requirementTypeValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type doesn't exist! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => NotBeAVoidedRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type is voided! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => BeAUniqueCodeAsync(command.RequirementTypeId, command.Code, token))
                .WithMessage(command => $"Another requirement type with this code already exists! Code={command.Code}")
                .MustAsync((command, token) => BeAUniqueTitleAsync(command.RequirementTypeId, command.Title, token))
                .WithMessage(command => $"Another requirement type with this title already exists! Title={command.Title}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(requirementTypeId, token);
            async Task<bool> NotBeAVoidedRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => !await requirementTypeValidator.IsVoidedAsync(requirementTypeId, token);
            async Task<bool> BeAUniqueCodeAsync(int requirementTypeId, string code, CancellationToken token)
                => !await requirementTypeValidator.ExistsWithSameCodeInAnotherTypeAsync(requirementTypeId, code, token);
            async Task<bool> BeAUniqueTitleAsync(int requirementTypeId, string title, CancellationToken token)
                => !await requirementTypeValidator.ExistsWithSameTitleInAnotherTypeAsync(requirementTypeId, title, token);
        }
    }
}
