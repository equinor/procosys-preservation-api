using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Tag
{
    public interface ITagValidator
    {
        bool Exists(int tagId);
        
        bool Exists(string tagNo, string projectName);

        bool IsVoided(int tagId);
        
        bool ProjectIsClosed(int tagId);
        
        bool VerifyPreservationStatus(int tagId, PreservationStatus status);
        
        bool HasANonVoidedRequirement(int tagId);
        
        bool AllRequirementDefinitionsExist(int tagId);

        bool ReadyToBePreserved(int tagId, DateTime preservedAtUtc);
        
        bool RequirementIsReadyForRecording(int tagId, int fieldId);
    }
}
