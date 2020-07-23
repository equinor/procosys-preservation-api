using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition;

namespace Equinor.Procosys.Preservation.Command.Validators.FieldValidators
{
    public interface IFieldValidator
    {
        Task<bool> ExistsAsync(int fieldId, CancellationToken token);
        Task<bool> IsVoidedAsync(int fieldId, CancellationToken token);
        Task<bool> IsValidForRecordingAsync(int fieldId, CancellationToken token);
        Task<bool> IsValidForAttachmentAsync(int fieldId, CancellationToken token);
        Task<bool> FieldTypeIsNotChanged(UpdateFieldsForCommand fieldToUpdate, CancellationToken token);
    }
}
