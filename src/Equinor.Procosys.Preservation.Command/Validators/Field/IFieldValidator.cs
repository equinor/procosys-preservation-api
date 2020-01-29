using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Field
{
    public interface IFieldValidator
    {
        bool Exists(int fieldId);
        
        bool IsVoided(int fieldId);
        
        bool VerifyFieldType(int fieldId, FieldType fieldType);
    }
}
