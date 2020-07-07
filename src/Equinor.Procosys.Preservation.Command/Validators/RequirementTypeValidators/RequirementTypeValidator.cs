﻿using System.Linq;
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

        public async Task<bool> RequirementDefinitionExistsAsync(int requirementTypeId, CancellationToken token)
        {
            var reqType = await _context.QuerySet<RequirementType>()
                .Include(rt => rt.RequirementDefinitions)
                .SingleOrDefaultAsync(rt => rt.Id == requirementTypeId, token);

            return reqType != null && reqType.RequirementDefinitions.Count != 0;
        }

        public async Task<bool> IsVoidedAsync(int requirementTypeId, CancellationToken token)
        {
            var reqType = await (from rt in _context.QuerySet<RequirementType>()
                where rt.Id == requirementTypeId
                select rt).SingleOrDefaultAsync(token);
            return reqType != null && reqType.IsVoided;
        }
    }
}
