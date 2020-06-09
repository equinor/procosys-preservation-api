using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag
{
    public class CreateAreaTagCommandValidator : AbstractValidator<CreateAreaTagCommand>
    {
        public CreateAreaTagCommandValidator(
            ITagValidator tagValidator,
            IStepValidator stepValidator,
            IProjectValidator projectValidator,
            IRequirementDefinitionValidator requirementDefinitionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
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
                    .MustAsync((_, requirements, token) => RequirementUsageIsForOtherAsync(requirements, token))
                    .WithMessage(command => "Requirements must include requirements to be used for other than suppliers!")
                    .MustAsync((_, requirements, token) => RequirementUsageIsNotForSupplierOnlyAsync(requirements, token))
                    .WithMessage(command => "Requirements can't include requirements just for suppliers!");
            });

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAnExistingAndClosedProjectAsync(command.ProjectName, token))
                .WithMessage(command => $"Project is closed! Project={command.ProjectName}")
                .MustAsync((command, token) => NotBeAnExistingTagWithinProjectAsync(command.GetTagNo(), command.ProjectName, token))
                .WithMessage(command => $"Tag already exists in scope for project! Tag={command.GetTagNo()} Project={command.ProjectName}")
                .MustAsync((command, token) => BeAnExistingStepAsync(command.StepId, token))
                .WithMessage(command => $"Step doesn't exists! Step={command.StepId}")
                .MustAsync((command, token) => NotBeAVoidedStepAsync(command.StepId, token))
                .WithMessage(command => $"Step is voided! Step={command.StepId}")
                .MustAsync((command, token) => BeASupplierStepAsync(command.StepId, token))
                .WithMessage(command => $"Step for a {TagType.PoArea.GetTagNoPrefix()} need to be for supplier!")
                    .When(command => command.TagType == TagType.PoArea, ApplyConditionTo.CurrentValidator);

            RuleForEach(command => command.Requirements)
                .MustAsync((_, req, __, token) => BeAnExistingRequirementDefinitionAsync(req, token))
                .WithMessage((_, req) => $"Requirement definition doesn't exists! Requirement={req.RequirementDefinitionId}")
                .MustAsync((_, req, __, token) => NotBeAVoidedRequirementDefinitionAsync(req, token))
                .WithMessage((_, req) => $"Requirement definition is voided! Requirement={req.RequirementDefinitionId}");

            async Task<bool> RequirementUsageIsForAllJourneysAsync(IEnumerable<RequirementForCommand> requirements, CancellationToken token)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return await requirementDefinitionValidator.UsageCoversBothForSupplierAndOtherAsync(reqIds, token);
            }                        

            async Task<bool> RequirementUsageIsForOtherAsync(IEnumerable<RequirementForCommand> requirements, CancellationToken token)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return await requirementDefinitionValidator.UsageCoversForOtherThanSuppliersAsync(reqIds, token);
            }                        

            async Task<bool> RequirementUsageIsNotForSupplierOnlyAsync(IEnumerable<RequirementForCommand> requirements, CancellationToken token)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return !await requirementDefinitionValidator.UsageCoversForSupplierOnlyAsync(reqIds, token);
            }                        

            bool BeUniqueRequirements(IEnumerable<RequirementForCommand> requirements)
            {
                var reqIds = requirements.Select(dto => dto.RequirementDefinitionId).ToList();
                return reqIds.Distinct().Count() == reqIds.Count;
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
