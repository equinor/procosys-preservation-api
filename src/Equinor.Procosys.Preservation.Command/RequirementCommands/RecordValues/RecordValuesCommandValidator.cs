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
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exists! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}")
                .MustAsync((command, token) => HaveRequirementWithActivePeriodAsync(command.TagId, command.RequirementId, token))
                .WithMessage(command =>
                    $"Tag doesn't have this requirement with active period! Tag={command.TagId}. Requirement={command.RequirementId}");

            When(command => command.FieldValues.Any(), () =>
            {
                RuleForEach(command => command.FieldValues)
                    .MustAsync((command, fv, token) => BeAFieldForRecordingAsync(fv.Key, token))
                    .WithMessage((command, fv) => $"Field values can not be recorded for field type! Field={fv.Key}")
                    .MustAsync((command, fv, token) => BeAValidValueForFieldAsync(fv.Key, fv.Value, token))
                    .WithMessage((command, fv) => $"Field value is not valid for field type! Field={fv.Key} Value={fv.Value}")
                    .MustAsync((command, fv, token) => BeAnExistingFieldAsync(fv.Key, token))
                    .WithMessage((command, fv) => $"Field doesn't exists! Field={fv.Key}")
                    .MustAsync((command, fv, token) => NotBeAVoidedFieldAsync(fv.Key, token))
                    .WithMessage((command, fv) => $"Field is voided! Field={fv.Key}");
            });
                        
            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);

            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);

            async Task<bool> HaveRequirementWithActivePeriodAsync(int tagId, int requirementId, CancellationToken token)
                => await tagValidator.HaveRequirementWithActivePeriodAsync(tagId, requirementId, token);

            async Task<bool> BeAnExistingFieldAsync(int fieldId, CancellationToken token)
                => await fieldValidator.ExistsAsync(fieldId, token);

            async Task<bool> NotBeAVoidedFieldAsync(int fieldId, CancellationToken token)
                => !await fieldValidator.IsVoidedAsync(fieldId, token);

            async Task<bool> BeAFieldForRecordingAsync(int fieldId, CancellationToken token)
                => await fieldValidator.IsValidForRecordingAsync(fieldId, token);

            async Task<bool> BeAValidValueForFieldAsync(int fieldId, string value, CancellationToken token)
                => await fieldValidator.IsValidValueAsync(fieldId, value, token);
        }
    }
}
