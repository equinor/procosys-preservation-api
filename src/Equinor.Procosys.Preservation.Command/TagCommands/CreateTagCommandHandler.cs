using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
    {
        private ITagRepository _tagRepository;

        public CreateTagCommandHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var tagToAdd = new Tag(request.TagNo, request.ProjectNo, request.Schema);
            _tagRepository.Add(tagToAdd);
            await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return tagToAdd.Id;
        }
    }
}
