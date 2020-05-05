using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTag
{
    public class UpdateTagCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public UpdateTagCommand(
            int tagId,
            string remark,
            string storageArea,
            ulong rowVersion)
        {
            TagId = tagId;
            Remark = remark;
            StorageArea = storageArea;
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public string Remark { get; }
        public string StorageArea { get; }
        public ulong RowVersion { get; }
    }
}
