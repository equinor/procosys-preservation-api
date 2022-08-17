using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition
{
    public class CreateRequirementDefinitionCommandValidator : AbstractValidator<CreateRequirementDefinitionCommand>
    {
        public CreateRequirementDefinitionCommandValidator(IRequirementTypeValidator requirementTypeValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => RequirementTypeMustExists(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type doesn't exist! Requirement type={command.RequirementTypeId}")
                .MustAsync((command, token) => RequirementTypeMustNotBeVoided(command.RequirementTypeId, token))
                .WithMessage(command => $"Requirement type is voided! Requirement type={command.RequirementTypeId}")
                .MustAsync((command, token) =>
                    RequirementDefinitionTitleMustBeUniqueOnType(command.RequirementTypeId, command.Title, command.Fields, token))
                .WithMessage(command => $"A requirement definition with this title already exists on the requirement type! Requirement type={command.RequirementTypeId}");

            async Task<bool> RequirementTypeMustExists(int reqTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(reqTypeId, token);

            async Task<bool> RequirementTypeMustNotBeVoided(int reqTypeId, CancellationToken token)
                => !await requirementTypeValidator.IsVoidedAsync(reqTypeId, token);

            async Task<bool> RequirementDefinitionTitleMustBeUniqueOnType(
                int reqTypeId, 
                string title, 
                IList<FieldsForCommand> fields, 
                CancellationToken token)
                => !await requirementTypeValidator.AnyRequirementDefinitionExistsWithSameTitleAsync(
                    reqTypeId, 
                    title, 
                    fields.Select(f => f.FieldType).Distinct().ToList(), 
                    token);
        }
    }
}
