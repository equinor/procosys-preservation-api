using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
{
    public class UpdateRequirementDefinitionCommand : IRequest<Result<string>>
    {
        public UpdateRequirementDefinitionCommand(
            int requirementTypeId,
            int requirementDefinitionId,
            int sortKey,
            RequirementUsage usage,
            string title,
            int defaultIntervalWeeks,
            string rowVersion,
            IList<Field> updatedFields,
            IList<Field> newFields)
        {
            RequirementTypeId = requirementTypeId;
            RowVersion = rowVersion;
            SortKey = sortKey;
            Title = title;
            //Code = code;
            //Icon = icon;
        }

        public int RequirementTypeId { get; }
        public string RowVersion { get; }
        public int SortKey { get; }
        public string Code { get; }
        public string Title { get; }
        public RequirementTypeIcon Icon { get; }
    }
}
