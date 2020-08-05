using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.PersonCommands.CreateSavedFilter
{
    public class CreateSavedFilterCommand : IRequest<Result<int>>
    {
        public CreateSavedFilterCommand(
            string title,
            string criteria)
        {
            Title = title;
            Criteria = criteria;
        }

        public string Title { get; }
        public string Criteria { get; }
    }
}
