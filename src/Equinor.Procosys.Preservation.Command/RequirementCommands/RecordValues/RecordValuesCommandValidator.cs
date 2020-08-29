using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.FieldValidators;
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
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => HasRequirementWithActivePeriodAsync(command.TagId, command.RequirementId, token))
                .WithMessage(command =>
                    $"Tag doesn't have this requirement with active period! Tag={command.TagId}. Requirement={command.RequirementId}");

            When(command => command.NumberValues.Any(), () =>
            {
                RuleForEach(command => command.NumberValues)
                    .MustAsync((_, fv, token) => BeAFieldForRecordingAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field values can not be recorded for field type! Field={fv.FieldId}")
                    .MustAsync((_, fv, token) => BeAnExistingFieldAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field doesn't exist! Field={fv.FieldId}")
                    .MustAsync((_, fv, token) => NotBeAVoidedFieldAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field is voided! Field={fv.FieldId}");
            });

            When(command => command.CheckBoxValues.Any(), () =>
            {
                RuleForEach(command => command.CheckBoxValues)
                    .MustAsync((_, fv, token) => BeAFieldForRecordingAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field values can not be recorded for field type! Field={fv.FieldId}")
                    .MustAsync((_, fv, token) => BeAnExistingFieldAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field doesn't exist! Field={fv.FieldId}")
                    .MustAsync((_, fv, token) => NotBeAVoidedFieldAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field is voided! Field={fv.FieldId}");
            });
                        
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> HasRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken token)
                => await tagValidator.HasRequirementWithActivePeriodAsync(tagId, requirementId, token);

            async Task<bool> BeAnExistingFieldAsync(int fieldId, CancellationToken token)
                => await fieldValidator.ExistsAsync(fieldId, token);

            async Task<bool> NotBeAVoidedFieldAsync(int fieldId, CancellationToken token)
                => !await fieldValidator.IsVoidedAsync(fieldId, token);

            async Task<bool> BeAFieldForRecordingAsync(int fieldId, CancellationToken token)
                => await fieldValidator.IsValidForRecordingAsync(fieldId, token);
        }
    }
}
