using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.FieldValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
{
    public class UpdateRequirementDefinitionCommandValidator : AbstractValidator<UpdateRequirementDefinitionCommand>
    {
        public UpdateRequirementDefinitionCommandValidator(
            IRequirementTypeValidator requirementTypeValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator,
            IFieldValidator fieldValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync(BeAnExistingRequirementDefinitionAsync)
                .WithMessage(_ => "Requirement type and/or requirement definition doesn't exist!")
                .MustAsync((command, token) => NotBeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is voided! Requirement definition={command.Title}")
                .MustAsync(RequirementDefinitionTitleMustBeUniqueOnType)
                .WithMessage(command => $"A requirement definition with this title already exists on the requirement type! Requirement type={command.Title}")
                .MustAsync((command, token) => AllFieldsToBeDeletedAreVoidedAsync(command.RequirementDefinitionId, command.UpdateFields, token))
                .WithMessage(command => $"Fields to be deleted must be voided! Requirement definition={command.Title}")
                .MustAsync((command, token) => NoFieldsToBeDeletedShouldBeInUseAsync(command.RequirementDefinitionId, command.UpdateFields, token))
                .WithMessage(command => $"Fields to be deleted can not be in use! Requirement definition={command.Title}");

            RuleForEach(command => command.UpdateFields)
                .MustAsync((command, field, _, token) => BeAnExistingField(command, field.Id, token))
                .WithMessage(_ => "Field doesn't exist in requirement!")
                .MustAsync((_, field, _, token) => BeSameFieldTypeOnExistingFieldsAsync(field, token))
                .WithMessage((_, field) => $"Cannot change field type on existing fields! Field={field.Id}");

            async Task<bool> BeAnExistingRequirementDefinitionAsync(UpdateRequirementDefinitionCommand command, CancellationToken token)
                => await requirementTypeValidator.RequirementDefinitionExistsAsync(command.RequirementTypeId, command.RequirementDefinitionId, token);

            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);

            async Task<bool> RequirementDefinitionTitleMustBeUniqueOnType(
                UpdateRequirementDefinitionCommand command,
                CancellationToken token)
            {
                var fieldTypesFromUpdated = command.UpdateFields.Select(uf => uf.FieldType).ToList();
                var fieldTypesFromNew = command.NewFields.Select(nf => nf.FieldType).ToList();

                return !await requirementTypeValidator.OtherRequirementDefinitionExistsWithSameTitleAsync(
                    command.RequirementTypeId,
                    command.RequirementDefinitionId,
                    command.Title,
                    fieldTypesFromUpdated.Concat(fieldTypesFromNew).Distinct().ToList(),
                    token);
            }

            async Task<bool> BeAnExistingField(UpdateRequirementDefinitionCommand command, int fieldId, CancellationToken token)
                => await requirementTypeValidator.FieldExistsAsync(command.RequirementTypeId, command.RequirementDefinitionId, fieldId, token);

            async Task<bool> BeSameFieldTypeOnExistingFieldsAsync(UpdateFieldsForCommand field, CancellationToken token)
                => await fieldValidator.VerifyFieldTypeAsync(field.Id, field.FieldType, token);

            async Task<bool> AllFieldsToBeDeletedAreVoidedAsync(int requirementDefinitionId, IList<UpdateFieldsForCommand> updateFields, CancellationToken token)
            {
                var updateFieldIds = updateFields.Select(u => u.Id).ToList();
                return await requirementDefinitionValidator.AllExcludedFieldsAreVoidedAsync(requirementDefinitionId, updateFieldIds, token);
            }

            async Task<bool> NoFieldsToBeDeletedShouldBeInUseAsync(int requirementDefinitionId, IList<UpdateFieldsForCommand> updateFields, CancellationToken token)
            {
                var updateFieldIds = updateFields.Select(u => u.Id).ToList();
                return !await requirementDefinitionValidator.AnyExcludedFieldsIsInUseAsync(requirementDefinitionId, updateFieldIds, token);
            }
        }
    }
}
