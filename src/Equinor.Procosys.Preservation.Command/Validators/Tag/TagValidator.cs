using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Tag
{
    public class TagValidator : ITagValidator
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IJourneyRepository _journeyRepository;

        public TagValidator(
            IProjectRepository projectRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IJourneyRepository journeyRepository)
        {
            _projectRepository = projectRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _journeyRepository = journeyRepository;
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

        public bool ReadyToBePreserved(int tagId, DateTime preservedAtUtc)
        {
            var tag = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            if (tag == null)
            {
                return false;
            }

            return tag.IsReadyToBePreserved(preservedAtUtc);
        }

        public bool HaveRequirementReadyForRecording(int tagId, int requirementId)
        {
            var tag = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            if (tag == null)
            {
                return false;
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == requirementId);

            return requirement != null && requirement.HasActivePeriod;
        }

        public bool HaveNextStep(int tagId)
        {
            var tag = _projectRepository.GetTagByTagIdAsync(tagId).Result;
            if (tag == null)
            {
                return false;
            }

            var journey = _journeyRepository.GetJourneyByStepIdAsync(tag.StepId).Result;
            var step = journey?.GetNextStep(tag.StepId);

            return step != null;
        }
    }
}
