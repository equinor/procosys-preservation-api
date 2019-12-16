using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class AllTagsQueryHandler : IRequestHandler<AllTagsQuery, IEnumerable<TagDto>>
    {
        private readonly ITagRepository _tagRepository;

        public AllTagsQueryHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<TagDto>> Handle(AllTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(x => new TagDto { Id = x.Id, Schema = x.Schema });
        }
    }
}
