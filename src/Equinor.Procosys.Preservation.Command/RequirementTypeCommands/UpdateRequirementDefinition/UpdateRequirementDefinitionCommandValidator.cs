using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.FieldValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
{
    public class UpdateRequirementDefinitionCommandValidator : AbstractValidator<UpdateRequirementDefinitionCommand>
    {
        public UpdateRequirementDefinitionCommandValidator(
            IRequirementTypeValidator requirementTypeValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator,
            IFieldValidator fieldValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type doesn't exist! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => BeAnExistingRequirementDefinitionAsync(command.RequirementTypeId, command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition doesn't exist! RequirementDefinition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotBeAVoidedRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type is voided! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => NotBeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is voided! RequirementDefinition={command.RequirementDefinitionId}")
                .MustAsync((command, token) 
                    => RequirementDefinitionTitleMustBeUniqueOnType(command.RequirementTypeId, command.RequirementDefinitionId, command.Title, command.UpdateFields, command.NewFields, token))
                .WithMessage(command => $"A requirement definition with this title already exists on the requirement type! RequirementType={command.RequirementTypeId}");

            RuleForEach(command => command.UpdateFields)
                .MustAsync((command, field, __, token) => BeAnExistingFieldToUpdate(field.Id, token))
                .WithMessage((_, field) => $"Field doesn't exist'! Field={field.Id}")
                .MustAsync((command, field, __, token) => BeSameFieldTypeOnExistingFieldsAsync(field, token))
                .WithMessage((_, field) => $"Cannot change field type on existing fields! Field={field.Id}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(requirementTypeId, token);
            
            async Task<bool> BeAnExistingRequirementDefinitionAsync(int requirementTypeId, int requirementDefinitionId, CancellationToken token)
                => await requirementTypeValidator.RequirementDefinitionExistsAsync(requirementTypeId, requirementDefinitionId, token);
            
            async Task<bool> NotBeAVoidedRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => !await requirementTypeValidator.IsVoidedAsync(requirementTypeId, token);
            
            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            
            async Task<bool> RequirementDefinitionTitleMustBeUniqueOnType(
                int reqTypeId, 
                int reqDefId,
                string title, 
                IList<UpdateFieldsForCommand> updatedFields, 
                IList<FieldsForCommand> newFields, 
                CancellationToken token)
            {
                var fieldTypesFromUpdated = updatedFields.Select(uf => uf.FieldType).ToList();
                var fieldTypesFromNew = newFields.Select(nf => nf.FieldType).ToList();

                return !await requirementTypeValidator.OtherRequirementDefinitionExistsWithSameTitleAsync(
                    reqTypeId,
                    reqDefId,
                    title,
                    fieldTypesFromUpdated.Concat(fieldTypesFromNew).Distinct().ToList(), 
                    token);
            }

            async Task<bool> BeAnExistingFieldToUpdate(int fieldId, CancellationToken token)
                => await fieldValidator.ExistsAsync(fieldId, token);

            async Task<bool> BeSameFieldTypeOnExistingFieldsAsync(UpdateFieldsForCommand field, CancellationToken token)
                => await fieldValidator.VerifyFieldTypeAsync(field.Id, field.FieldType, token);
        }
    }
}
