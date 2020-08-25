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
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotExistsARequirementTypeWithSameCode(command.Code, token))
                .WithMessage("Requirement type with this code already exists!")
                .MustAsync((command, token) => NotExistsARequirementTypeWithSameTitle(command.Title, token))
                .WithMessage("Requirement type with this title already exists!");

            async Task<bool> NotExistsARequirementTypeWithSameCode(string code, CancellationToken token)
                => !await requirementTypeValidator.ExistsWithSameCodeAsync(code, token);

            async Task<bool> NotExistsARequirementTypeWithSameTitle(string title, CancellationToken token)
                => !await requirementTypeValidator.ExistsWithSameTitleAsync(title, token);
        }
    }
}
