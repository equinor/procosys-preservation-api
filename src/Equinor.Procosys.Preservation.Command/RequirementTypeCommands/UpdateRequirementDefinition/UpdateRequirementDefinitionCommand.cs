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
            IList<UpdateFieldsForCommand> updatedFields,
            IList<FieldsForCommand> newFields)
        {
            RequirementTypeId = requirementTypeId;
            RequirementDefinitionId = requirementDefinitionId;
            Title = title;
            SortKey = sortKey;
            Usage = usage;
            RowVersion = rowVersion;
            DefaultIntervalWeeks = defaultIntervalWeeks;
            UpdateFields = updatedFields;
            NewFields = newFields;
        }

        public int RequirementTypeId { get; }
        public int RequirementDefinitionId { get; }
        public int SortKey { get; }
        public string Title { get; }
        public int DefaultIntervalWeeks { get; }
        public RequirementUsage Usage { get; }
        public string RowVersion { get; }
        public IList<UpdateFieldsForCommand> UpdateFields { get; }
        public IList<FieldsForCommand> NewFields { get; }
    }
}
