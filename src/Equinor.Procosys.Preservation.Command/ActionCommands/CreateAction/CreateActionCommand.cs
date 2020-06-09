using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction
{
    public class CreateActionCommand : IRequest<Result<int>>, ITagCommandRequest
    {
        public CreateActionCommand(int tagId, string title, string description, DateTime? dueTimeUtc, Guid currentUserOid)
        {
            TagId = tagId;
            Title = title;
            Description = description;
            DueTimeUtc = dueTimeUtc;
            CurrentUserOid = currentUserOid;
        }
        public int TagId { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
        public Guid CurrentUserOid { get; }
    }
}
