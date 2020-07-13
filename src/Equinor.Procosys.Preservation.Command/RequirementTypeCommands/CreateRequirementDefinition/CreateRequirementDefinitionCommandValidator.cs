using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition
{
    public class CreateRequirementDefinitionCommandValidator : AbstractValidator<CreateRequirementDefinitionCommand>
    {
        public CreateRequirementDefinitionCommandValidator(IRequirementDefinitionValidator requirementDefinitionValidator, IRequirementTypeValidator requirementTypeValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((command, token) => RequirementTypeMustExists(command.RequirementTypeId, token))
                .WithMessage("Requirement type doesn't exists!")
                .MustAsync((command, token) => RequirementTypeMustNotBeVoided(command.RequirementTypeId, token))
                .WithMessage("Requirement type is voided!")
                .MustAsync((command, token) =>
                    RequirementDefinitionTitleMustBeUniqueOnType(command.RequirementTypeId, command.Title, command.Fields, token))
                .WithMessage("A requirement definition with this title already exists on the requirement type");

            async Task<bool> RequirementTypeMustExists(int reqTypeId, CancellationToken token)
                => await requirementTypeValidator.ExistsAsync(reqTypeId, token);

            async Task<bool> RequirementTypeMustNotBeVoided(int reqTypeId, CancellationToken token)
                => !await requirementTypeValidator.IsVoidedAsync(reqTypeId, token);

            async Task<bool> RequirementDefinitionTitleMustBeUniqueOnType(int reqTypeId, string title, IEnumerable<Field> fields, CancellationToken token)
                => !await requirementDefinitionValidator.IsNotUniqueTitleOnRequirementTypeAsync(reqTypeId, title, fields, token);
        }
    }
}
