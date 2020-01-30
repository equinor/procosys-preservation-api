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
            => _projectRepository.GetTagByTagIdAsync(tagId).Result != null;

        public bool Exists(string tagNo, string projectName)
            => _projectRepository.GetTagByTagNoAsync(tagNo, projectName).Result != null;

        public bool IsVoided(int tagId)
        {
            var tag = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            return tag != null && tag.IsVoided;
        }

        public bool ProjectIsClosed(int tagId)
        {
            var project = _projectRepository.GetByTagIdAsync(tagId).Result;

            return project != null && project.IsClosed;
        }

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

        public bool ReadyToBePreserved(int tagId)
        {
            var tag = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            if (tag == null)
            {
                return false;
            }


            return tag.ReadyToBePreserved;
        }

        public bool RequirementIsReadyForRecording(int tagId, int fieldId)
        {
            var tag = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            var reqDef = _requirementTypeRepository.GetRequirementDefinitionByFieldIdAsync(fieldId).Result;
            if (tag == null || reqDef == null)
            {
                return false;
            }

            var req = tag.Requirements.Single(r => r.RequirementDefinitionId == reqDef.Id);
            return req.HasActivePeriod;
        }
    }
}
