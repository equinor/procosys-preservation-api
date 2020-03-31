using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues
{
    public class RecordValuesCommand : IRequest<Result<Unit>>
    {
        public RecordValuesCommand(string plant, int tagId, int requirementId, Dictionary<int, string> fieldValues, string comment)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            TagId = tagId;
            RequirementId = requirementId;
            FieldValues = fieldValues ?? new Dictionary<int, string>();
            Comment = comment;
        }

        public string Plant { get; }
        public int TagId { get; }
        public int RequirementId { get; }
        public Dictionary<int, string> FieldValues { get; }
        public string Comment { get; }
    }
}
