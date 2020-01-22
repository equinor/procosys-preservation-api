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

        public bool Exists(string tagNo, string projectName)
            => _tagRepository.GetByNoAsync(tagNo, projectName).Result != null;

        public bool IsVoided(int tagId)
        {
            var r = _tagRepository.GetByIdAsync(tagId).Result;
            return r != null && r.IsVoided;
        }

        public bool ProjectIsClosed(int tagId) => false; // todo
    }
}
