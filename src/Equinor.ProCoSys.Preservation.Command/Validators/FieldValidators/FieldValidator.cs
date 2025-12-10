using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.Validators.FieldValidators
{
    public class FieldValidator : IFieldValidator
    {
        private readonly IReadOnlyContext _context;

        public FieldValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> IsVoidedAsync(int fieldId, CancellationToken token)
        {
            var field = await (from f in _context.QuerySet<Field>()
                               where f.Id == fieldId
                               select f).SingleOrDefaultAsync(token);
            return field != null && field.IsVoided;
        }

        public async Task<bool> IsValidForRecordingAsync(int fieldId, CancellationToken token)
        {
            var field = await (from f in _context.QuerySet<Field>()
                               where f.Id == fieldId
                               select f).SingleOrDefaultAsync(token);
            return field != null && (field.FieldType == FieldType.Number || field.FieldType == FieldType.CheckBox);
        }

        public async Task<bool> IsValidForAttachmentAsync(int fieldId, CancellationToken token)
        {
            var field = await (from f in _context.QuerySet<Field>()
                               where f.Id == fieldId
                               select f).SingleOrDefaultAsync(token);
            return field != null && (field.FieldType == FieldType.Attachment);
        }

        public async Task<bool> VerifyFieldTypeAsync(int fieldId, FieldType fieldType, CancellationToken token)
        {
            var field = await (from f in _context.QuerySet<Field>()
                               where f.Id == fieldId
                               select f).SingleOrDefaultAsync(token);

            return field != null && field.FieldType == fieldType;
        }
    }
}
