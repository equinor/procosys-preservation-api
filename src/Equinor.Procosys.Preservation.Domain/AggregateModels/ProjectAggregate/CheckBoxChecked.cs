using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    // A field needing input can be of 3 types: Number, CheckBox and Attachment
    // This class represent a checked CheckBox
    public class CheckBoxChecked : FieldValue
    {
        protected CheckBoxChecked()
        {
        }

        public CheckBoxChecked(string schema, Field field)
            :base(schema, field)
        {
        }
    }
}
