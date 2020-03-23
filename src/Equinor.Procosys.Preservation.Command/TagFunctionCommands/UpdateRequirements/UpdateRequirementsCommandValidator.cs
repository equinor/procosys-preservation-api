using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagFunctionCommands.UpdateRequirements
{
    public class UpdateRequirementsCommandValidator : AbstractValidator<UpdateRequirementsCommand>
    {
        public UpdateRequirementsCommandValidator(IRequirementDefinitionValidator requirementDefinitionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            RuleFor(command => command.Requirements)
                .Must(BeUniqueRequirements)
                .WithMessage("Requirement definitions must be unique!");

            RuleForEach(command => command.Requirements)
                .MustAsync((_, req, __, token) => BeAnExistingRequirementDefinitionAsync(req, token))
                .WithMessage((_, req) =>
                    $"Requirement definition doesn't exists! Requirement={req.RequirementDefinitionId}")
                .MustAsync((_, req, __, token) => NotBeAVoidedRequirementDefinitionAsync(req, token))
                .WithMessage((_, req) =>
                    $"Requirement definition is voided! Requirement={req.RequirementDefinitionId}");

            async Task<bool> BeAnExistingRequirementDefinitionAsync(Requirement requirement, CancellationToken token)
                => await requirementDefinitionValidator.ExistsAsync(requirement.RequirementDefinitionId, token);

            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(Requirement requirement, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirement.RequirementDefinitionId, token);
                        
            bool BeUniqueRequirements(IEnumerable<Requirement> requirements)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
            }
        }
    }
}
