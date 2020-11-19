using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.FieldValidators
{
    public interface IFieldValidator
    {
        [Obsolete]
        Task<bool> ExistsAsync(int fieldId, CancellationToken token);
        Task<bool> IsVoidedAsync(int fieldId, CancellationToken token);
        Task<bool> IsValidForRecordingAsync(int fieldId, CancellationToken token);
        Task<bool> IsValidForAttachmentAsync(int fieldId, CancellationToken token);
        Task<bool> VerifyFieldTypeAsync(int fieldId, FieldType fieldType, CancellationToken token);
    }
}
