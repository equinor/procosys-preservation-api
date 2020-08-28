﻿using System.Collections.Generic;
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
                .WithMessage(command => $"Requirement type doesn't exist! Requirement type={command.RequirementTypeId}")
                .MustAsync((command, token) => BeAnExistingRequirementDefinitionAsync(command.RequirementTypeId, command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition doesn't exist! Requirement definition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotBeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is voided! Requirement definition={command.Title}")
                .MustAsync(RequirementDefinitionTitleMustBeUniqueOnType)
                .WithMessage(command => $"A requirement definition with this title already exists on the requirement type! Requirement type={command.Title}")
                .MustAsync((command, token) => AllFieldsToBeDeletedAreVoidedAsync(command.RequirementDefinitionId, command.UpdateFields, token))
                .WithMessage(command => $"Fields to be deleted must be voided! Requirement definition={command.Title}")
                .MustAsync((command, token) => NoFieldsToBeDeletedShouldBeInUseAsync(command.RequirementDefinitionId, command.UpdateFields, token))
                .WithMessage(command => $"Fields to be deleted can not be in use! Requirement definition={command.Title}");

            RuleForEach(command => command.UpdateFields)
                .MustAsync((command, field, __, token) => BeAnExistingField(field.Id, token))
                .WithMessage((_, field) => $"Field doesn't exist! Field={field.Id}")
                .MustAsync((command, field, __, token) => BeSameFieldTypeOnExistingFieldsAsync(field, token))
                .WithMessage((_, field) => $"Cannot change field type on existing fields! Field={field.Id}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(requirementTypeId, token);
            
            async Task<bool> BeAnExistingRequirementDefinitionAsync(int requirementTypeId, int requirementDefinitionId, CancellationToken token)
                => await requirementTypeValidator.RequirementDefinitionExistsAsync(requirementTypeId, requirementDefinitionId, token);
            
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

            async Task<bool> BeAnExistingField(int fieldId, CancellationToken token)
                => await fieldValidator.ExistsAsync(fieldId, token);

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
