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

            When(command => command.Requirements.Any(), () =>
            {
                RuleFor(command => command.Requirements)
                    .Must(BeUniqueRequirements)
                    .WithMessage("Requirement definitions must be unique!")
                    .MustAsync((_, requirements, token) => RequirementUsageIsForAllJourneysAsync(requirements, token))
                    .WithMessage(command => "Requirements must include requirements to be used both for supplier and other than suppliers!");

                RuleForEach(command => command.Requirements)
                    .MustAsync((_, req, __, token) => BeAnExistingRequirementDefinitionAsync(req, token))
                    .WithMessage((_, req) =>
                        $"Requirement definition doesn't exists! Requirement={req.RequirementDefinitionId}")
                    .MustAsync((_, req, __, token) => NotBeAVoidedRequirementDefinitionAsync(req, token))
                    .WithMessage((_, req) =>
                        $"Requirement definition is voided! Requirement={req.RequirementDefinitionId}");
            });

            async Task<bool> BeAnExistingRequirementDefinitionAsync(RequirementForCommand requirement, CancellationToken token)
                => await requirementDefinitionValidator.ExistsAsync(requirement.RequirementDefinitionId, token);

            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(RequirementForCommand requirement, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirement.RequirementDefinitionId, token);
                        
            bool BeUniqueRequirements(IEnumerable<RequirementForCommand> requirements)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
            }
            
            async Task<bool> RequirementUsageIsForAllJourneysAsync(IEnumerable<RequirementForCommand> requirements, CancellationToken token)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return await requirementDefinitionValidator.UsageCoversBothForSupplierAndOtherAsync(reqIds, token);
            }                        
        }
    }
}
