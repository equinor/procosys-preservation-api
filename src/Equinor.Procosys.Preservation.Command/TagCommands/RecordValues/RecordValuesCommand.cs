using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordValues
{
    public class RecordValuesCommand : IRequest<Result<Unit>>
    {
        public RecordValuesCommand(int tagId, int requirementId, Dictionary<int, string> fieldValues, string comment)
        {
            TagId = tagId;
            RequirementId = requirementId;
            FieldValues = fieldValues ?? new Dictionary<int, string>();
            Comment = comment;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public Dictionary<int, string> FieldValues { get; }
        public string Comment { get; }
    }
}
