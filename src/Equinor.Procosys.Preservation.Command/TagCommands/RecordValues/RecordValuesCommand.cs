using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordValues
{
    public class RecordValuesCommand : IRequest<Result<Unit>>
    {
        public RecordValuesCommand(int tagId, int requirementDefinitionId, List<FieldValue> fieldValues, string comment)
        {
            TagId = tagId;
            RequirementDefinitionId = requirementDefinitionId;
            FieldValues = fieldValues;
            Comment = comment;
        }

        public int TagId { get; }
        public int RequirementDefinitionId { get; }
        public List<FieldValue> FieldValues { get; }
        public string Comment { get; }
    }
}
