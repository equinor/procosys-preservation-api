using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.PersonCommands.CreateSavedFilter
{
    public class CreateSavedFilterCommand : IRequest<Result<int>>
    {
        public CreateSavedFilterCommand(
            string title,
            string criteria,
            bool defaultFilter)
        {
            Title = title;
            Criteria = criteria;
            DefaultFilter = defaultFilter;
        }

        public string Title { get; }
        public string Criteria { get; }
        public bool DefaultFilter { get; }
    }
}
