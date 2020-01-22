using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Tag
{
    public class TagValidator : ITagValidator
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public TagValidator(IProjectRepository projectRepository, IRequirementTypeRepository requirementTypeRepository)
        {
            _projectRepository = projectRepository;
            _requirementTypeRepository = requirementTypeRepository;
        }

        public bool Exists(int tagId)
            => _projectRepository.GetByIdAsync(tagId).Result != null;

        public bool Exists(string tagNo, string projectName)
            => _projectRepository.GetTagByTagNoAsync(tagNo, projectName).Result != null;

        public bool IsVoided(int tagId)
        {
            var r = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            return r != null && r.IsVoided;
        }

        public bool ProjectIsClosed(int tagId) => false; // todo

        public bool VerifyPreservationStatus(int tagId, PreservationStatus status)
        {
            var tag = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            return tag != null && tag.Status == status;
        }

        public bool HasANonVoidedRequirement(int tagId)
        {
            var tag = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            return tag?.Requirements != null && tag.Requirements.Any(r => !r.IsVoided);
        }

        public bool AllRequirementDefinitionsExist(int tagId)
        {
            var tag = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            if (tag?.Requirements == null)
            {
                return true;
            }

            var reqDefIds = tag.Requirements.Select(r => r.RequirementDefinitionId).Distinct().ToList();
            var reqDefs = _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds).Result;
            return reqDefs.Count == reqDefIds.Count;
        }
    }
}
