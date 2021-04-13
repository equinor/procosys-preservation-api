using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Validators.FieldValidators
{
    public interface IFieldValidator
    {
        Task<bool> IsVoidedAsync(int fieldId, CancellationToken token);
        Task<bool> IsValidForRecordingAsync(int fieldId, CancellationToken token);
        Task<bool> IsValidForAttachmentAsync(int fieldId, CancellationToken token);
        Task<bool> VerifyFieldTypeAsync(int fieldId, FieldType fieldType, CancellationToken token);
    }
}
