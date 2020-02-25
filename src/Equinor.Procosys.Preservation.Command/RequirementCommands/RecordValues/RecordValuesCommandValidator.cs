using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
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
                .MustAsync((command, token) => BeAnExistingTag(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exists! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTag(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => HaveRequirementWithActivePeriod(command.TagId, command.RequirementId, token))
                .WithMessage(command =>
                    $"Tag doesn't have this requirement with active period! Tag={command.TagId}. Requirement={command.RequirementId}");

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
                        
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingTag(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTag(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> HaveRequirementWithActivePeriod(int tagId, int requirementId, CancellationToken token)
                => await tagValidator.HaveRequirementWithActivePeriodAsync(tagId, requirementId, token);

            bool BeAnExistingField(KeyValuePair<int, string> fieldValue) => fieldValidator.Exists(fieldValue.Key);

            bool NotBeAVoidedField(KeyValuePair<int, string>  fieldValue) => !fieldValidator.IsVoided(fieldValue.Key);

            bool BeAFieldForRecording(KeyValuePair<int, string>  fieldValue)
                => fieldValidator.IsValidForRecording(fieldValue.Key);

            bool BeAValidValueForField(KeyValuePair<int, string>  fieldValue)
                => fieldValidator.IsValidValue(fieldValue.Key, fieldValue.Value);
        }
    }
}
