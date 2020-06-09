using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTag
{
    public class UpdateTagCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public UpdateTagCommand(
            int tagId,
            string remark,
            string storageArea,
            string rowVersion,
            Guid currentUserOid)
        {
            TagId = tagId;
            Remark = remark;
            StorageArea = storageArea;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }

        public int TagId { get; }
        public string Remark { get; }
        public string StorageArea { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
