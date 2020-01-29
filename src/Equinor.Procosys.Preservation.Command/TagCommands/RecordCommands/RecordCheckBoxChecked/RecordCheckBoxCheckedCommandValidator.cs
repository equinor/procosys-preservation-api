using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
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

            RuleFor(command => command).SetValidator(
                new RecordCommandValidator<RecordCheckBoxCheckedCommand>(tagValidator, fieldValidator));

            RuleFor(command => command)
                .Must(BeOfTypeCheckBox)
                .WithMessage(command => $"Field is not of type {FieldType.CheckBox}! FieldId={command.FieldId}");

            bool BeOfTypeCheckBox(RecordCheckBoxCheckedCommand command)
                => fieldValidator.VerifyFieldType(command.FieldId, FieldType.CheckBox);
        }
    }
}
