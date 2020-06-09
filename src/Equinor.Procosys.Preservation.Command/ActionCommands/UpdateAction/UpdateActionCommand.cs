using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction
{
    public class UpdateActionCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public UpdateActionCommand(
            int tagId,
            int actionId,
            string title,
            string description,
            DateTime? dueTimeUtc,
            string rowVersion,
            Guid currentUserOid)
        {
            TagId = tagId;
            ActionId = actionId;
            Title = title;
            Description = description;
            DueTimeUtc = dueTimeUtc;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }
        public int TagId { get; }
        public int ActionId { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
