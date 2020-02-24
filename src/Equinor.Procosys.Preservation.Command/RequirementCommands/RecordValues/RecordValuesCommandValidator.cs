using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.Project;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues
{
    public class RecordValuesCommandValidator : AbstractValidator<RecordValuesCommand>
    {
        public RecordValuesCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            IFieldValidator fieldValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .Must(command => BeAnExistingTag(command.TagId))
                .WithMessage(command => $"Tag doesn't exists! Tag={command.TagId}")
                .Must(command => NotBeAVoidedTag(command.TagId))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .Must(command => HaveRequirementReadyForRecording(command.TagId, command.RequirementId))
                .WithMessage(command =>
                    $"Tag doesn't have this requirement ready for recording! Tag={command.TagId}. Requirement={command.RequirementId}");

            When(command => command.FieldValues.Any(), () =>
            {
                RuleForEach(command => command.FieldValues)
                    .Must(BeAFieldForRecording)
                    .WithMessage((command, fv) => $"Field values can not be recorded for field type! Field={fv.Key}")
                    .Must(BeAValidValueForField)
                    .WithMessage((command, fv) => $"Field value is not valid for field type! Field={fv.Key} Value={fv.Value}")
                    .Must(BeAnExistingField)
                    .WithMessage((command, fv) => $"Field doesn't exists! Field={fv.Key}")
                    .Must(NotBeAVoidedField)
                    .WithMessage((command, fv) => $"Field is voided! Field={fv.Key}");
            });
                        
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken cancellationToken)
                => !await projectValidator.IsClosedForTagAsync(tagId, cancellationToken);

            bool BeAnExistingTag(int tagId) => tagValidator.Exists(tagId);

            bool NotBeAVoidedTag(int tagId) => !tagValidator.IsVoided(tagId);

            bool BeAnExistingField(KeyValuePair<int, string> fieldValue) => fieldValidator.Exists(fieldValue.Key);

            bool NotBeAVoidedField(KeyValuePair<int, string>  fieldValue) => !fieldValidator.IsVoided(fieldValue.Key);

            bool HaveRequirementReadyForRecording(int tagId, int requirementId)
                => tagValidator.HaveRequirementReadyForRecording(tagId, requirementId);

            bool BeAFieldForRecording(KeyValuePair<int, string>  fieldValue)
                => fieldValidator.IsValidForRecording(fieldValue.Key);

            bool BeAValidValueForField(KeyValuePair<int, string>  fieldValue)
                => fieldValidator.IsValidValue(fieldValue.Key, fieldValue.Value);
        }
    }
}
