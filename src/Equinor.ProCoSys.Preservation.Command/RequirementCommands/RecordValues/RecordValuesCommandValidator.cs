using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.FieldValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.RequirementCommands.RecordValues
{
    public class RecordValuesCommandValidator : AbstractValidator<RecordValuesCommand>
    {
        public RecordValuesCommandValidator(
            IProjectValidator projectValidator,
            ITagValidator tagValidator,
            IFieldValidator fieldValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync(BeAnExistingRequirementAsync)
                .WithMessage(_ => "Tag and/or requirement doesn't exist!")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => HasRequirementWithActivePeriodAsync(command.TagId, command.RequirementId, token))
                .WithMessage(command =>
                    $"Tag doesn't have this requirement with active period! Tag={command.TagId}. Requirement={command.RequirementId}");

            When(command => command.NumberValues.Any(), () =>
            {
                RuleForEach(command => command.NumberValues)
                    .MustAsync((command, fv, token) => BeAnExistingFieldForRequirementAsync(command, fv.FieldId, token))
                    .WithMessage(_ => "Field doesn't exist in requirement!")
                    .MustAsync((_, fv, token) => BeAFieldForRecordingAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field values can not be recorded for field type! Field={fv.FieldId}")
                    .MustAsync((_, fv, token) => NotBeAVoidedFieldAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field is voided! Field={fv.FieldId}");
            });

            When(command => command.CheckBoxValues.Any(), () =>
            {
                RuleForEach(command => command.CheckBoxValues)
                    .MustAsync((command, fv, token) => BeAnExistingFieldForRequirementAsync(command, fv.FieldId, token))
                    .WithMessage(_ => "Field doesn't exist in requirement!")
                    .MustAsync((_, fv, token) => BeAFieldForRecordingAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field values can not be recorded for field type! Field={fv.FieldId}")
                    .MustAsync((_, fv, token) => NotBeAVoidedFieldAsync(fv.FieldId, token))
                    .WithMessage((_, fv) => $"Field is voided! Field={fv.FieldId}");
            });
                        
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingRequirementAsync(RecordValuesCommand command, CancellationToken token)
                => await tagValidator.ExistsRequirementAsync(command.TagId, command.RequirementId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> HasRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken token)
                => await tagValidator.HasRequirementWithActivePeriodAsync(tagId, requirementId, token);

            async Task<bool> BeAnExistingFieldForRequirementAsync(RecordValuesCommand command, int fieldId, CancellationToken token)
                => await tagValidator.ExistsFieldForRequirementAsync(command.TagId, command.RequirementId, fieldId, token);

            async Task<bool> NotBeAVoidedFieldAsync(int fieldId, CancellationToken token)
                => !await fieldValidator.IsVoidedAsync(fieldId, token);

            async Task<bool> BeAFieldForRecordingAsync(int fieldId, CancellationToken token)
                => await fieldValidator.IsValidForRecordingAsync(fieldId, token);
        }
    }
}
