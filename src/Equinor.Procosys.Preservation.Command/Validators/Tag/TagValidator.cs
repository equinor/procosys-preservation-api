using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Tag
{
    public class TagValidator : ITagValidator
    {
        private readonly ITagRepository _tagRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public TagValidator(ITagRepository tagRepository, IRequirementTypeRepository requirementTypeRepository)
        {
            _tagRepository = tagRepository;
            _requirementTypeRepository = requirementTypeRepository;
        }

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

        public bool AllRequirementDefinitionsExists(int tagId)
        {
            var tag = _tagRepository.GetByIdAsync(tagId).Result;
            if (tag == null || tag.Requirements == null)
            {
                return true;
            }

            var reqDefIds = tag.Requirements.Select(r => r.RequirementDefinitionId).Distinct().ToList();
            var reqDefs = _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds).Result;
            return reqDefs.Count == reqDefIds.Count;
        }
    }
}
