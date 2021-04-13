using System;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ActionCommands.CreateAction
{
    public class CreateActionCommand : IRequest<Result<int>>, ITagCommandRequest
    {
        public CreateActionCommand(int tagId, string title, string description, DateTime? dueTimeUtc)
        {
            TagId = tagId;
            Title = title;
            Description = description;
            DueTimeUtc = dueTimeUtc;
        }
        public int TagId { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
    }
}
