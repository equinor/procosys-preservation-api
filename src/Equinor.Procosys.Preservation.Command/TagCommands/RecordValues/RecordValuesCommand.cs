using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordValues
{
    public class RecordValuesCommand : IRequest<Result<Unit>>
    {
        public RecordValuesCommand(int tagId, int requirementId, List<FieldValue> fieldValues, string comment)
        {
            TagId = tagId;
            RequirementId = requirementId;
            FieldValues = fieldValues ?? new List<FieldValue>();
            Comment = comment;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public List<FieldValue> FieldValues { get; }
        public string Comment { get; }
    }
}
