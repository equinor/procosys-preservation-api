using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode
{
    public class DeleteModeCommandValidator : AbstractValidator<DeleteModeCommand>
    {
        public DeleteModeCommandValidator(IModeRepository modeRepository)
        {
            RuleFor(x => x.ModeId)
                .ModeMustExist(modeRepository);
        }
    }
}
