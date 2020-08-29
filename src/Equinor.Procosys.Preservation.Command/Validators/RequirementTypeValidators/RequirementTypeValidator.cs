using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators
{
    public class RequirementTypeValidator : IRequirementTypeValidator
    {
        private readonly IReadOnlyContext _context;

        public RequirementTypeValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int requirementTypeId, CancellationToken token) =>
            await (from rt in _context.QuerySet<RequirementType>()
                where rt.Id == requirementTypeId
                select rt).AnyAsync(token);

        public async Task<bool> AnyRequirementDefinitionExistsAsync(int requirementTypeId, CancellationToken token)
        {
            var reqType = await GetRequirementTypeWithDefinitionsAsync(requirementTypeId, token);

            return reqType != null && reqType.RequirementDefinitions.Any();
        }

        public async Task<bool> HasRequirementDefinitionAsync(int requirementTypeId, int requirementDefinitionId, CancellationToken token)
        {
            var reqType = await GetRequirementTypeWithDefinitionsAsync(requirementTypeId, token);

            return reqType != null && reqType.RequirementDefinitions.SingleOrDefault(d => d.Id == requirementDefinitionId) != null;
        }

        public async Task<bool> IsVoidedAsync(int requirementTypeId, CancellationToken token)
        {
            var reqType = await (from rt in _context.QuerySet<RequirementType>()
                where rt.Id == requirementTypeId
                select rt).SingleOrDefaultAsync(token);
            return reqType != null && reqType.IsVoided;
        }

        public async Task<bool> ExistsWithSameCodeAsync(string code, CancellationToken token) =>
            await (from rt in _context.QuerySet<RequirementType>()
                where rt.Code == code
                select rt).AnyAsync(token);

        public async Task<bool> ExistsWithSameTitleAsync(string title, CancellationToken token) =>
            await (from rt in _context.QuerySet<RequirementType>()
                where rt.Title == title
                select rt).AnyAsync(token);

        public async Task<bool> ExistsWithSameCodeInAnotherTypeAsync(int requirementTypeId, string code, CancellationToken token) =>
            await (from rt in _context.QuerySet<RequirementType>()
                where rt.Code == code && rt.Id != requirementTypeId
                select rt).AnyAsync(token);

        public async Task<bool> ExistsWithSameTitleInAnotherTypeAsync(int requirementTypeId, string title, CancellationToken token) =>
            await (from rt in _context.QuerySet<RequirementType>()
                where rt.Title == title && rt.Id != requirementTypeId
                select rt).AnyAsync(token);

        public async Task<bool> AnyRequirementDefinitionExistsWithSameTitleAsync(
            int requirementTypeId,
            string reqDefTitle,
            IList<FieldType> fieldTypes,
            CancellationToken token)
        {
            var needsUserInput = fieldTypes != null && fieldTypes.Any(ft => ft.NeedsUserInput());

            var reqType = await _context.QuerySet<RequirementType>()
                .Include(rt => rt.RequirementDefinitions)
                .ThenInclude(r => r.Fields)
                .SingleOrDefaultAsync(rt => rt.Id == requirementTypeId, token);

            var reqDefinitions = reqType.RequirementDefinitions;

            return reqDefinitions.Any(rd => rd.Title == reqDefTitle && rd.NeedsUserInput == needsUserInput);
        }

        public async Task<bool> OtherRequirementDefinitionExistsWithSameTitleAsync(
            int requirementTypeId,
            int requirementDefinitionId,
            string reqDefTitle,
            IList<FieldType> fieldTypes,
            CancellationToken token)
        {
            var needsUserInput = fieldTypes != null && fieldTypes.Any(ft => ft.NeedsUserInput());

            var reqType = await _context.QuerySet<RequirementType>()
                .Include(rt => rt.RequirementDefinitions)
                .ThenInclude(r => r.Fields)
                .SingleOrDefaultAsync(rt => rt.Id == requirementTypeId, token);

            var reqDefinitions = reqType.RequirementDefinitions;

            return reqDefinitions.Any(rd => rd.Id != requirementDefinitionId && rd.Title == reqDefTitle && rd.NeedsUserInput == needsUserInput);
        }

        private async Task<RequirementType> GetRequirementTypeWithDefinitionsAsync(int requirementTypeId, CancellationToken token)
        {
            var reqType = await _context.QuerySet<RequirementType>()
                .Include(rt => rt.RequirementDefinitions)
                .SingleOrDefaultAsync(rt => rt.Id == requirementTypeId, token);
            return reqType;
        }
    }
}
