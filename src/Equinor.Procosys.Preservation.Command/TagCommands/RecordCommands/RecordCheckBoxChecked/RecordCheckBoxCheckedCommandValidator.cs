using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands.RecordCheckBoxChecked
{
    public class RecordCheckBoxCheckedCommandValidator :  AbstractValidator<RecordCheckBoxCheckedCommand>
    {
        public RecordCheckBoxCheckedCommandValidator(
            ITagValidator tagValidator,
            IFieldValidator fieldValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(r => r).SetValidator(
                new RecordCommandValidator<RecordCheckBoxCheckedCommand>(tagValidator, fieldValidator));
        }
    }
}
