using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Tag
{
    public class TagValidator : ITagValidator
    {
        private readonly ITagRepository _tagRepository;

        public TagValidator(ITagRepository tagRepository)
            => _tagRepository = tagRepository;

        public bool Exists(int tagId)
            => _tagRepository.GetByIdAsync(tagId).Result != null;

        public bool Exists(string tagNo, string projectNo)
            => _tagRepository.GetByNoAsync(tagNo, projectNo).Result != null;

        public bool IsVoided(int tagId)
        {
            var r = _tagRepository.GetByIdAsync(tagId).Result;
            return r != null && r.IsVoided;
        }

        public bool ProjectIsClosed(int tagId) => false; // todo

        public bool VerifyPreservationStatus(int tagId, PreservationStatus status)
        {
            var tag = _tagRepository.GetByIdAsync(tagId).Result;
            return tag != null && tag.Status == status;
        }

        public bool HasANonVoidedRequirement(int tagId)
        {
            var tag = _tagRepository.GetByIdAsync(tagId).Result;
            return tag?.Requirements != null && tag.Requirements.Any(r => !r.IsVoided);
        }
    }
}
