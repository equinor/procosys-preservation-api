using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction
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
        public int TagId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueTimeUtc { get; set; }
    }
}
