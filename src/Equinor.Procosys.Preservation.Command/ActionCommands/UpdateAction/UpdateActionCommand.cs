using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction
{
    public class UpdateActionCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public UpdateActionCommand(
            int tagId,
            int actionId,
            string title,
            string description,
            DateTime? dueTimeUtc)
        {
            TagId = tagId;
            ActionId = actionId;
            Title = title;
            Description = description;
            DueTimeUtc = dueTimeUtc;
        }
        public int TagId { get; }
        public int ActionId { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
    }
}
