using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements
{
    public class UpdateTagStepAndRequirementsCommandValidator : AbstractValidator<UpdateTagStepAndRequirementsCommand>
    {
        public UpdateTagStepAndRequirementsCommandValidator(
             IProjectValidator projectValidator,
             ITagValidator tagValidator,
             IStepValidator stepValidator,
             IRequirementDefinitionValidator requirementDefinitionValidator)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .Must((command) => BeUniqueRequirements(command.UpdatedRequirements, command.NewRequirements))
                .WithMessage(command => "Requirement definitions must be unique!");

            WhenAsync((command, token) => BeASupplierStepAsync(command.StepId, token), () =>
            {
                RuleFor(command => command)
                    .MustAsync((_, command, token) => RequirementUsageIsForAllJourneysAsync(command.UpdatedRequirements, command.NewRequirements, token))
                    .WithMessage(command => "Requirements must include requirements to be used both for supplier and other than suppliers!");
            }).Otherwise(() =>
            {
                RuleFor(command => command)
                    .MustAsync((_, command, token) => RequirementUsageIsForJourneysWithoutSupplierAsync(command.UpdatedRequirements, command.NewRequirements, token))
                    .WithMessage(command => "Requirements must include requirements to be used for other than suppliers!")
                    .MustAsync((_, command, token) => RequirementUsageIsNotForSupplierStepOnlyAsync(command.UpdatedRequirements, command.NewRequirements, token))
                    .WithMessage(command => "Requirements can't include requirements just for suppliers!");
            });

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAVoidedTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag is voided! Tag={command.TagId}");


            async Task<bool> BeASupplierStepAsync(int stepId, CancellationToken token)
                => await stepValidator.IsForSupplierAsync(stepId, token);

            bool BeUniqueRequirements(IList<UpdateRequirementForCommand> updatedRequirements, IList<RequirementForCommand> newRequirements)
            {
                var updatedReqIds = updatedRequirements.Select(u => u.RequirementDefinitionId).ToList();
                var newReqIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();

                if ((updatedReqIds.Distinct().Count() != updatedRequirements.Count) ||
                    (newReqIds.Distinct().Count() != newRequirements.Count))
                {
                    return false;
                }

                return updatedReqIds.Intersect(newReqIds).Any()==false;

            }

            async Task<bool> RequirementUsageIsForAllJourneysAsync(IList<UpdateRequirementForCommand> updatedRequirements, IList<RequirementForCommand> newRequirements, CancellationToken token)
            {
                var updatedReqIds = updatedRequirements.Select(u => u.RequirementDefinitionId).ToList();
                var newReqIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();
                var allReqIds = updatedReqIds.Union(newReqIds).ToList();

                return (allReqIds.Count==0) || (await requirementDefinitionValidator.UsageCoversBothForSupplierAndOtherAsync(allReqIds, token));
            }

            async Task<bool> RequirementUsageIsForJourneysWithoutSupplierAsync(IList<UpdateRequirementForCommand> updatedRequirements, IList<RequirementForCommand> newRequirements, CancellationToken token)
            {
                var updatedReqIds = updatedRequirements.Select(u => u.RequirementDefinitionId).ToList();
                var newReqIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();
                var allReqIds = updatedReqIds.Union(newReqIds).ToList();

                return (allReqIds.Count == 0) || await requirementDefinitionValidator.UsageCoversForOtherThanSuppliersAsync(updatedReqIds.Union(newReqIds).ToList(), token);
            }

            async Task<bool> RequirementUsageIsNotForSupplierStepOnlyAsync(IList<UpdateRequirementForCommand> updatedRequirements, IList<RequirementForCommand> newRequirements, CancellationToken token)
            {
                var updatedReqIds = updatedRequirements.Select(u => u.RequirementDefinitionId).ToList();
                var newReqIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();
                var allReqIds = updatedReqIds.Union(newReqIds).ToList();

                return (allReqIds.Count == 0) || !await requirementDefinitionValidator.UsageCoversForSupplierOnlyAsync(updatedReqIds.Union(newReqIds).ToList(), token);
            }

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
        }
    }
}
