using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
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

        public async Task<bool> IsVoidedAsync(
            int requirementDefinitionId,
            CancellationToken token)
        {
            var reqDef = await (from rd in _context.QuerySet<RequirementDefinition>()
                where rd.Id == requirementDefinitionId
                select rd).SingleOrDefaultAsync(token);
            return reqDef != null && reqDef.IsVoided;
        }

        public async Task<bool> UsageCoversBothForSupplierAndOtherAsync(
            List<int> requirementDefinitionIds,
            CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitions(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForAll)
                   || (reqDefs.Any(rd => rd.Usage == RequirementUsage.ForSuppliersOnly) &&
                       reqDefs.Any(rd => rd.Usage == RequirementUsage.ForOtherThanSuppliers));
        }

        public async Task<bool> UsageCoversForOtherThanSuppliersAsync(
            List<int> requirementDefinitionIds,
            CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitions(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForAll) ||
                   reqDefs.Any(rd => rd.Usage == RequirementUsage.ForOtherThanSuppliers);
        }

        public async Task<bool> HasAnyForSupplierOnlyUsageAsync(
            List<int> requirementDefinitionIds,
            CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitions(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForSuppliersOnly);
        }

        private async Task<List<RequirementDefinition>> GetRequirementDefinitions(
            List<int> requirementDefinitionIds,
            CancellationToken token)
            => await (from rd in _context.QuerySet<RequirementDefinition>()
                where requirementDefinitionIds.Contains(rd.Id)
                select rd).ToListAsync(token);

        public async Task<bool> FieldsExistAsync(int requirementDefinitionId, CancellationToken token)
        {
            var reqDef = await (from rd in _context.QuerySet<RequirementDefinition>()
                where rd.Id == requirementDefinitionId
                select rd).SingleOrDefaultAsync(token);
            return reqDef != null && reqDef.Fields.Count > 0;
        }

        public async Task<bool> TagRequirementsExistAsync(int requirementDefinitionId, CancellationToken token)
             => await (from tr in _context.QuerySet<TagRequirement>()
                    where tr.RequirementDefinitionId == requirementDefinitionId
                    select tr).AnyAsync(token);

        public async Task<bool> TagFunctionRequirementsExistAsync(int requirementDefinitionId, CancellationToken token)
            => await (from tfr in _context.QuerySet<TagFunctionRequirement>()
                where tfr.RequirementDefinitionId == requirementDefinitionId
                select tfr).AnyAsync(token);
    }
}
