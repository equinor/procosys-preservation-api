using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators
{
    public class RequirementDefinitionValidator : IRequirementDefinitionValidator
    {
        private readonly IReadOnlyContext _context;

        public RequirementDefinitionValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int requirementDefinitionId, CancellationToken token) =>
            await (from rd in _context.QuerySet<RequirementDefinition>()
                where rd.Id == requirementDefinitionId
                select rd).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int requirementDefinitionId, CancellationToken token)
        {
            var reqDef = await (from rd in _context.QuerySet<RequirementDefinition>()
                where rd.Id == requirementDefinitionId
                select rd).SingleOrDefaultAsync(token);
            return reqDef != null && reqDef.IsVoided;
        }

        public async Task<bool> UsageCoversBothForSupplierAndOtherAsync(List<int> requirementDefinitionIds, CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitions(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForAll)
                   || (reqDefs.Any(rd => rd.Usage == RequirementUsage.ForSuppliersOnly) &&
                       reqDefs.Any(rd => rd.Usage == RequirementUsage.ForOtherThanSuppliers));
        }

        public async Task<bool> UsageCoversForOtherThanSuppliersAsync(List<int> requirementDefinitionIds, CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitions(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForAll) ||
                   reqDefs.Any(rd => rd.Usage == RequirementUsage.ForOtherThanSuppliers);
        }

        public async Task<bool> UsageCoversForSupplierOnlyAsync(List<int> requirementDefinitionIds, CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitions(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForSuppliersOnly);
        }

        public async Task<bool> BeUniqueRequirements(IEnumerable<int> updatedRequirements, IEnumerable<int> newRequirements, CancellationToken token)
        {
            var updatedReqDefIds = await GetAllUpdatedReqDefIds(updatedRequirements, token);

            var newReqDefIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();

            if (newReqDefIds.Distinct().Count() != newRequirements.Count)
            {
                return false;
            }

            return updatedReqDefIds.Intersect(newReqDefIds).Any() == false;
        }

        public async Task<bool> RequirementUsageIsForAllJourneysAsync(IEnumerable<int> updatedRequirements, IEnumerable<int> newRequirements, CancellationToken token)
        {
            var updatedReqDefIds = await GetAllUpdatedReqDefIds(updatedRequirements, token);

            var newReqDefIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();
            var allReqDefIds = updatedReqDefIds.Union(newReqDefIds).ToList();

            return (allReqDefIds.Count == 0) || (await UsageCoversBothForSupplierAndOtherAsync(allReqDefIds, token));
        }

        public async Task<bool> RequirementUsageIsForJourneysWithoutSupplierAsync(IEnumerable<int> updatedRequirements, IEnumerable<int> newRequirements, CancellationToken token)
        {
            var updatedReqDefIds = await GetAllUpdatedReqDefIds(updatedRequirements, token);

            var newReqDefIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();
            var allReqDefIds = updatedReqDefIds.Union(newReqDefIds).ToList();

            return (allReqDefIds.Count == 0) || await UsageCoversForOtherThanSuppliersAsync(allReqDefIds, token);
        }

        public async Task<bool> RequirementUsageIsNotForSupplierStepOnlyAsync(IEnumerable<int> updatedRequirements, IEnumerable<int> newRequirements, CancellationToken token)
        {
            var updatedReqDefIds = await GetAllUpdatedReqDefIds(updatedRequirements, token);

            var newReqDefIds = newRequirements.Select(r => r.RequirementDefinitionId).ToList();
            var allReqDefIds = updatedReqDefIds.Union(newReqDefIds).ToList();

            return (allReqDefIds.Count == 0) || !await UsageCoversForSupplierOnlyAsync(allReqDefIds, token);
        }

        public async Task<List<int>> GetAllUpdatedReqDefIds(IEnumerable<int> updatedTagReqIds, CancellationToken token)
        {
            var updatedReqDefIds = await (from req in _context.QuerySet<TagRequirement>()
                                          where updatedTagReqIds.Contains(req.Id)
                                          select req.RequirementDefinitionId).ToListAsync(token);

            return updatedReqDefIds;
        }
        private async Task<List<RequirementDefinition>> GetRequirementDefinitions(
            List<int> requirementDefinitionIds,
            CancellationToken token)
            => await (from rd in _context.QuerySet<RequirementDefinition>()
                where requirementDefinitionIds.Contains(rd.Id)
                select rd).ToListAsync(token);
    }
}
