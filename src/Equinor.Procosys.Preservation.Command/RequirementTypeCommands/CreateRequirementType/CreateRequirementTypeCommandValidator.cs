using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementType
{
    public class CreateRequirementTypeCommandValidator : AbstractValidator<CreateRequirementTypeCommand>
    {
        public CreateRequirementTypeCommandValidator(IRequirementTypeValidator requirementTypeValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => NotExistsARequirementTypeWithSameCode(command.Code, token))
                .WithMessage(command => $"Requirement type with this code already exists!")
                .MustAsync((command, token) => NotExistsARequirementTypeWithSameTitle(command.Title, token))
                .WithMessage(command => $"Requirement type with this title already exists!");

            async Task<bool> NotExistsARequirementTypeWithSameCode(string code, CancellationToken token)
                => !await requirementTypeValidator.IsNotUniqueCodeAsync(code, token);

            async Task<bool> NotExistsARequirementTypeWithSameTitle(string title, CancellationToken token)
                => !await requirementTypeValidator.IsNotUniqueTitleAsync(title, token);
        }
    }
}
