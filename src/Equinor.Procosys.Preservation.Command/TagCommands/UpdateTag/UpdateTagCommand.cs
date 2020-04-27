using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTag
{
    public class UpdateTagCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public UpdateTagCommand(
            int tagId,
            string remark,
            string storageArea)
        {
            TagId = tagId;
            Remark = remark;
            StorageArea = storageArea;
        }

        public int TagId { get; }
        public string Remark { get; }
        public string StorageArea { get; }
    }
}
