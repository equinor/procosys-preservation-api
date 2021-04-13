using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.CreateAreaTag
{
    public class CreateAreaTagCommandValidator : AbstractValidator<CreateAreaTagCommand>
    {
        public CreateAreaTagCommandValidator(
            ITagValidator tagValidator,
            IStepValidator stepValidator,
            IProjectValidator projectValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator)
        {
            CascadeMode = CascadeMode.Stop;
            
            WhenAsync((command, token) => BeASupplierStepAsync(command.StepId, token), () =>
            {
                RuleFor(command => command.Requirements)
                    .Must(BeUniqueRequirements)
                    .WithMessage(command => "Requirement definitions must be unique!")
                    .MustAsync((_, requirements, token) => RequirementUsageIsForAllJourneysAsync(requirements, token))
                    .WithMessage(command => "Requirements must include requirements to be used both for supplier and other than suppliers!");
            }).Otherwise(() =>
            {
                RuleFor(command => command.Requirements)
                    .Must(BeUniqueRequirements)
                    .WithMessage(command => "Requirement definitions must be unique!")
                    .MustAsync((_, requirements, token) => RequirementUsageIsForJourneysWithoutSupplierAsync(requirements, token))
                    .WithMessage(command => "Requirements must include requirements to be used for other than suppliers!")
                    .MustAsync((_, requirements, token) => RequirementUsageIsNotForSupplierStepOnlyAsync(requirements, token))
                    .WithMessage(command => "Requirements can not include requirements just for suppliers!");
            });

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAnExistingAndClosedProjectAsync(command.ProjectName, token))
                .WithMessage(command => $"Project is closed! Project={command.ProjectName}")
                .MustAsync((command, token) => NotBeAnExistingTagWithinProjectAsync(command.GetTagNo(), command.ProjectName, token))
                .WithMessage(command => $"Tag already exists in scope for project! Tag={command.GetTagNo()}")
                .MustAsync((command, token) => BeAnExistingStepAsync(command.StepId, token))
                .WithMessage(command => $"Step doesn't exist! Step={command.StepId}")
                .MustAsync((command, token) => NotBeAVoidedStepAsync(command.StepId, token))
                .WithMessage(command => $"Step is voided! Step={command.StepId}")
                .MustAsync((command, token) => BeASupplierStepAsync(command.StepId, token))
                .WithMessage(command => $"Step for a {TagType.PoArea.GetTagNoPrefix()} need to be for supplier!")
                    .When(command => command.TagType == TagType.PoArea, ApplyConditionTo.CurrentValidator);

            RuleForEach(command => command.Requirements)
                .MustAsync((_, req, __, token) => BeAnExistingRequirementDefinitionAsync(req, token))
                .WithMessage((_, req) => $"Requirement definition doesn't exist! Requirement definition={req.RequirementDefinitionId}")
                .MustAsync((_, req, __, token) => NotBeAVoidedRequirementDefinitionAsync(req, token))
                .WithMessage((_, req) => $"Requirement definition is voided! Requirement definition={req.RequirementDefinitionId}");

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

            async Task<bool> RequirementUsageIsForJourneysWithoutSupplierAsync(IEnumerable<RequirementForCommand> requirements, CancellationToken token)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return await requirementDefinitionValidator.UsageCoversForOtherThanSuppliersAsync(reqIds, token);
            }                        

            async Task<bool> RequirementUsageIsNotForSupplierStepOnlyAsync(IEnumerable<RequirementForCommand> requirements, CancellationToken token)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return !await requirementDefinitionValidator.HasAnyForSupplierOnlyUsageAsync(reqIds, token);
            }                        

            async Task<bool> NotBeAnExistingAndClosedProjectAsync(string projectName, CancellationToken token)
                => !await projectValidator.IsExistingAndClosedAsync(projectName, token);

            async Task<bool> NotBeAnExistingTagWithinProjectAsync(string tagNo, string projectName, CancellationToken token)
                => !await tagValidator.ExistsAsync(tagNo, projectName, token);

            async Task<bool> BeAnExistingStepAsync(int stepId, CancellationToken token)
                => await stepValidator.ExistsAsync(stepId, token);

            async Task<bool> NotBeAVoidedStepAsync(int stepId, CancellationToken token)
                => !await stepValidator.IsVoidedAsync(stepId, token);

            async Task<bool> BeASupplierStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsForSupplierAsync(stepId, token);

            async Task<bool> BeAnExistingRequirementDefinitionAsync(RequirementForCommand requirement, CancellationToken token)
                => await requirementDefinitionValidator.ExistsAsync(requirement.RequirementDefinitionId, token);

            async Task<bool> NotBeAVoidedRequirementDefinitionAsync(RequirementForCommand requirement, CancellationToken token)
                => !await requirementDefinitionValidator.IsVoidedAsync(requirement.RequirementDefinitionId, token);
        }
    }
}
