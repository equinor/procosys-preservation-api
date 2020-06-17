using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements
{
    public class UpdateTagStepAndRequirementsCommandValidator : AbstractValidator<UpdateTagStepAndRequirementsCommand>
    {
        private readonly IReadOnlyContext _context;

        public UpdateTagStepAndRequirementsCommandValidator(
             IProjectValidator projectValidator,
             ITagValidator tagValidator,
             IStepValidator stepValidator,
             IRequirementDefinitionValidator requirementDefinitionValidator,
             IReadOnlyContext context)
        {
            _context = context;
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(command => command)
                .MustAsync((_, command, token) => BeUniqueRequirements(command.UpdatedRequirements, command.NewRequirements, token))
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

            async Task<bool> BeUniqueRequirements(IList<UpdateRequirementForCommand> updatedRequirements, IList<RequirementForCommand> newRequirements, CancellationToken token)
            {
                var updatedReqDefIds = await GetAllUpdatedReqDefIds(updatedRequirements, token);

                var newReqDefIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();

                if (newReqDefIds.Distinct().Count() != newRequirements.Count)
                {
                    return false;
                }

                return updatedReqDefIds.Intersect(newReqDefIds).Any()==false;

            }

            async Task<bool> RequirementUsageIsForAllJourneysAsync(IList<UpdateRequirementForCommand> updatedRequirements, IList<RequirementForCommand> newRequirements, CancellationToken token)
            {
                var updatedReqDefIds = await GetAllUpdatedReqDefIds(updatedRequirements, token);

                var newReqDefIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();
                var allReqDefIds = updatedReqDefIds.Union(newReqDefIds).ToList();

                return (allReqDefIds.Count==0) || (await requirementDefinitionValidator.UsageCoversBothForSupplierAndOtherAsync(allReqDefIds, token));
            }

            async Task<bool> RequirementUsageIsForJourneysWithoutSupplierAsync(IList<UpdateRequirementForCommand> updatedRequirements, IList<RequirementForCommand> newRequirements, CancellationToken token)
            {
                var updatedReqDefIds = await GetAllUpdatedReqDefIds(updatedRequirements, token);

                var newReqDefIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();
                var allReqDefIds = updatedReqDefIds.Union(newReqDefIds).ToList();

                return (allReqDefIds.Count == 0) || await requirementDefinitionValidator.UsageCoversForOtherThanSuppliersAsync(allReqDefIds, token);
            }

            async Task<bool> RequirementUsageIsNotForSupplierStepOnlyAsync(IList<UpdateRequirementForCommand> updatedRequirements, IList<RequirementForCommand> newRequirements, CancellationToken token)
            {
                var updatedReqDefIds = await GetAllUpdatedReqDefIds(updatedRequirements, token);

                var newReqDefIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();
                var allReqDefIds = updatedReqDefIds.Union(newReqDefIds).ToList();

                return (allReqDefIds.Count == 0) || !await requirementDefinitionValidator.UsageCoversForSupplierOnlyAsync(allReqDefIds, token);
            }

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
            async Task<bool> BeAnExistingTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);
            async Task<bool> NotBeAVoidedTagAsync(int tagId, CancellationToken token)
                => !await tagValidator.IsVoidedAsync(tagId, token);
        }

        private async Task<List<int>> GetAllUpdatedReqDefIds(IList<UpdateRequirementForCommand> updatedRequirements, CancellationToken token)
        {
            var updatedReqIds = updatedRequirements.Select(u => u.RequirementId);
            var updatedReqDefIds = await (from req in _context.QuerySet<TagRequirement>()
                where updatedReqIds.Contains(req.Id)
                select req.RequirementDefinitionId).ToListAsync(token);

            return updatedReqDefIds;
        }
    }
}
