using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues
{
    public class RecordValuesCommand : IRequest<Result<Unit>>
    {
        public RecordValuesCommand(
            int tagId,
            int requirementId,
            List<NumberFieldValue> numberValues,
            List<CheckBoxFieldValue> checkBoxValues,
            string comment)
        {
            TagId = tagId;
            RequirementId = requirementId;
            NumberValues = numberValues ?? new List<NumberFieldValue>();
            CheckBoxValues = checkBoxValues ?? new List<CheckBoxFieldValue>();
            Comment = comment;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public List<NumberFieldValue> NumberValues { get; }
        public List<CheckBoxFieldValue> CheckBoxValues { get; }
        public string Comment { get; }
    }
}
