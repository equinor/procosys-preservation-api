using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands
{
    public abstract class RecordCommand : IRequest<Result<Unit>>
    {
        protected RecordCommand(int tagId, int fieldId)
        {
            TagId = tagId;
            FieldId = fieldId;
        }

        public int TagId { get; }
        public int FieldId { get; }
    }
}
