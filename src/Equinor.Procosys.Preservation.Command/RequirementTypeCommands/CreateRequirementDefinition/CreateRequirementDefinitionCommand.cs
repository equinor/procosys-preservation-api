using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition
{
    public class CreateRequirementDefinitionCommand : IRequest<Result<int>>
    {
        public CreateRequirementDefinitionCommand(
            int id,
            int sortKey,
            RequirementUsage usage,
            string title,
            int defaultIntervalWeeks,
            IEnumerable<Field> fields = null)
        {
            RequirementTypeId = id;
            SortKey = sortKey;
            Usage = usage;
            Title = title;
            DefaultIntervalWeeks = defaultIntervalWeeks;
            Fields = fields;
        }

        public int RequirementTypeId { get; }
        public int SortKey { get; }
        public RequirementUsage Usage { get; }
        public string Title { get; }
        public int DefaultIntervalWeeks { get; }
        public IEnumerable<Field> Fields { get; }
    }
}
