using System;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction
{
    public class UpdateActionCommand : IRequest<Result<Unit>>, ITagRequest
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
        public int TagId { get; set; }
        public int ActionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueTimeUtc { get; set; }
    }
}
