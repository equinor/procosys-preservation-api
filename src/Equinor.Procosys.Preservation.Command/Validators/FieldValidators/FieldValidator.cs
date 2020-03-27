using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Command.Validators.FieldValidators
{
    public class FieldValidator : IFieldValidator
    {
        private readonly IReadOnlyContext _context;

        public FieldValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(int fieldId, CancellationToken token) =>
            await (from f in _context.QuerySet<Field>()
                where f.Id == fieldId
                select f).AnyAsync(token);

        public async Task<bool> IsVoidedAsync(int fieldId, CancellationToken token)
        {
            var field = await (from f in _context.QuerySet<Field>()
                where f.Id == fieldId
                select f).FirstOrDefaultAsync(token);
            return field != null && field.IsVoided;
        }

        public async Task<bool> IsValidValueAsync(int fieldId, string value, CancellationToken token)
        {
            var field = await (from f in _context.QuerySet<Field>()
                where f.Id == fieldId
                select f).FirstOrDefaultAsync(token);
            switch (field.FieldType)
            {
                case FieldType.Number:
                    return NumberValue.IsValidValue(value, out _);
                case FieldType.CheckBox:
                    return CheckBoxChecked.IsValidValue(value, out _);
                default:
                    return false;
            }
        }

        public async Task<bool> IsValidForRecordingAsync(int fieldId, CancellationToken token)
        {
            var field = await (from f in _context.QuerySet<Field>()
                where f.Id == fieldId
                select f).FirstOrDefaultAsync(token);
            return field != null && (field.FieldType == FieldType.Number || field.FieldType == FieldType.CheckBox);
        }
    }
}
