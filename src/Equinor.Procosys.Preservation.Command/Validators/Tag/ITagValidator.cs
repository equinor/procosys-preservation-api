using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Tag
{
    public interface ITagValidator
    {
        bool Exists(int tagId);
        
        bool Exists(string tagNo, string projectNo);

        bool IsVoided(int tagId);
        
        bool ProjectIsClosed(int tagId);
        
        bool VerifyPreservationStatus(int tagId, PreservationStatus status);
        
        bool HasANonVoidedRequirement(int tagId);
    }
}
