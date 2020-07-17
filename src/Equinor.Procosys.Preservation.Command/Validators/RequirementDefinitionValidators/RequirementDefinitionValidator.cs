using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands;
using Equinor.Procosys.Preservation.Domain;
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

        public async Task<bool> IsNotUniqueTitleOnRequirementTypeAsync(
            int requirementTypeId,
            string reqDefTitle,
            IList<FieldType> fieldTypes,
            CancellationToken token)
        {
            var needsUserInput = fieldTypes.Any(ft => ft == FieldType.Number ||
                                                 ft == FieldType.Attachment ||
                                                 ft == FieldType.CheckBox);
            var reqType = await _context.QuerySet<RequirementType>()
                .Include(rt => rt.RequirementDefinitions)
                .ThenInclude(r => r.Fields)
                .SingleOrDefaultAsync(rt => rt.Id == requirementTypeId, token);

            var reqDefinitions = reqType.RequirementDefinitions;

            return reqDefinitions.Any(rd => rd.Title == reqDefTitle && rd.NeedsUserInput == needsUserInput);
        }
    }
}
