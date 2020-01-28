using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands.RecordCheckBoxChecked
{
    public class RecordCheckBoxCheckedCommandValidator :  AbstractValidator<RecordCheckBoxCheckedCommand>
    {
        public RecordCheckBoxCheckedCommandValidator(RecordCommandValidator<RecordCheckBoxCheckedCommand> recordCommandValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(r => r).SetValidator(recordCommandValidator);
        }
    }
}
