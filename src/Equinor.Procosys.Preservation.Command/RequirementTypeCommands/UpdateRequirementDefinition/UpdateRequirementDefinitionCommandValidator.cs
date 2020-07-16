using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
{
    public class UpdateRequirementDefinitionCommandValidator : AbstractValidator<UpdateRequirementDefinitionCommand>
    {
        public UpdateRequirementDefinitionCommandValidator(
            IRequirementTypeValidator requirementTypeValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => BeAnExistingRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type doesn't exist! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => BeAnExistingRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition doesn't exist! RequirementDefinition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => NotBeAVoidedRequirementTypeAsync(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type is voided! RequirementType={command.RequirementTypeId}")
                .MustAsync((command, token) => NotBeAVoidedRequirementDefinitionAsync(command.RequirementDefinitionId, token))
                .WithMessage(command => $"Requirement definition is voided! RequirementDefinition={command.RequirementDefinitionId}")
                .MustAsync((command, token) => RequirementDefinitionTitleMustBeUniqueOnType(command.RequirementTypeId, command.Title, command.UpdateFields, command.NewFields, token))
                .WithMessage(command => $"A requirement definition with this title already exists on the requirement type! RequirementType={command.RequirementTypeId}");

            async Task<bool> BeAnExistingRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(requirementTypeId, token);
            async Task<bool> BeAnExistingRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => await requirementDefinitionValidator.ExistsAsync(requirementDefinitionId, token);
            async Task<bool> NotBeAVoidedRequirementTypeAsync(int requirementTypeId, CancellationToken token)
                => !await requirementTypeValidator.IsVoidedAsync(requirementTypeId, token);
            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(int requirementDefinitionId, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirementDefinitionId, token);
            async Task<bool> RequirementDefinitionTitleMustBeUniqueOnType(
                int reqTypeId, 
                string title, 
                IList<UpdateFieldsForCommand> updatedFields, 
                IList<FieldsForCommand> newFields, 
                CancellationToken token)
            {

                var fieldTypes1 = updatedFields.Select(uf => uf.FieldType).ToList();
                var fieldTypes2 = newFields.Select(nf => nf.FieldType).ToList();

                return !await requirementDefinitionValidator.IsNotUniqueTitleOnRequirementTypeAsync(
                    reqTypeId, 
                    title,
                    fieldTypes1.Concat(fieldTypes2).ToList(), 
                    token);
            }
        }
    }
}
