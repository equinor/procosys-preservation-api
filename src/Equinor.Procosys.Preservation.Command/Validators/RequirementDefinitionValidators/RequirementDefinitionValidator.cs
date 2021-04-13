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

        public async Task<bool> ExistsFieldAsync(int requirementDefinitionId, int fieldId, CancellationToken token) =>
            await (from rd in _context.QuerySet<RequirementDefinition>()
                join f in _context.QuerySet<Field>() on rd.Id equals EF.Property<int>(f, "RequirementDefinitionId")
                where rd.Id == requirementDefinitionId && f.Id == fieldId
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
            var reqDefs = await GetRequirementDefinitionsAsync(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForAll)
                   || (reqDefs.Any(rd => rd.Usage == RequirementUsage.ForSuppliersOnly) &&
                       reqDefs.Any(rd => rd.Usage == RequirementUsage.ForOtherThanSuppliers));
        }

        public async Task<bool> UsageCoversForSuppliersAsync(List<int> requirementDefinitionIds, CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitionsAsync(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForAll) ||
                   reqDefs.Any(rd => rd.Usage == RequirementUsage.ForSuppliersOnly);
        }

        public async Task<bool> UsageCoversForOtherThanSuppliersAsync(
            List<int> requirementDefinitionIds,
            CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitionsAsync(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForAll) ||
                   reqDefs.Any(rd => rd.Usage == RequirementUsage.ForOtherThanSuppliers);
        }

        public async Task<bool> HasAnyForSupplierOnlyUsageAsync(
            List<int> requirementDefinitionIds,
            CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitionsAsync(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForSuppliersOnly);
        }

        public async Task<bool> HasAnyForForOtherThanSuppliersUsageAsync(List<int> requirementDefinitionIds, CancellationToken token)
        {
            var reqDefs = await GetRequirementDefinitionsAsync(requirementDefinitionIds, token);
            return reqDefs.Any(rd => rd.Usage == RequirementUsage.ForOtherThanSuppliers);
        }

        public async Task<bool> HasAnyFieldsAsync(int requirementDefinitionId, CancellationToken token)
        {
            var reqDef = await GetRequirementDefinitionWithFieldsAsync(requirementDefinitionId, token);
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

        public async Task<bool> AllExcludedFieldsAreVoidedAsync(int requirementDefinitionId, List<int> updateFieldIds, CancellationToken token)
        {
            var reqDef = await GetRequirementDefinitionWithFieldsAsync(requirementDefinitionId, token);
            if (reqDef == null || !reqDef.Fields.Any())
            {
                return true;
            }

            var excludedFields = reqDef.Fields.Where(f => !updateFieldIds.Contains(f.Id)).ToList();

            return excludedFields.All(f => f.IsVoided);
        }

        public async Task<bool> AnyExcludedFieldsIsInUseAsync(int requirementDefinitionId, List<int> updateFieldIds, CancellationToken token)
        {
            var reqDef = await GetRequirementDefinitionWithFieldsAsync(requirementDefinitionId, token);
            if (reqDef == null || !reqDef.Fields.Any())
            {
                return false;
            }

            var excludedFieldIds = reqDef.Fields.Where(f => !updateFieldIds.Contains(f.Id)).Select(f => f.Id).ToList();

            if (!excludedFieldIds.Any())
            {
                return false;
            }

            return await (from fv in _context.QuerySet<FieldValue>()
                where excludedFieldIds.Contains(fv.FieldId)
                select fv).AnyAsync(token);
        }

        private async Task<RequirementDefinition> GetRequirementDefinitionWithFieldsAsync(int requirementDefinitionId, CancellationToken token)
            => await (from rd in _context.QuerySet<RequirementDefinition>()
                    .Include(rd => rd.Fields)
                where rd.Id == requirementDefinitionId
                select rd).SingleOrDefaultAsync(token);

        private async Task<List<RequirementDefinition>> GetRequirementDefinitionsAsync(List<int> requirementDefinitionIds, CancellationToken token)
            => await (from rd in _context.QuerySet<RequirementDefinition>()
                where requirementDefinitionIds.Contains(rd.Id)
                select rd).ToListAsync(token);
    }
}
