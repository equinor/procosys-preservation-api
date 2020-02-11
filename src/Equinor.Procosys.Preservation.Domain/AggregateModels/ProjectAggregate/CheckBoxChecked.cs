using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    // A field needing input can be of 3 types: Number, CheckBox and Attachment
    // This class represent a CHECKED CheckBox field,
    // I.e:
    //      If table row exists -> end user has checked the checkbox for particular field and saved
    //      If end user uncheck checkbox for particular field, and save, table row will be deleted
    public class CheckBoxChecked : FieldValue
    {
        protected CheckBoxChecked()
        {
        }

        public CheckBoxChecked(string schema, Field field)
            :base(schema, field)
        {
        }

        public static bool IsValidValue(string value, out bool isChecked)
            => bool.TryParse(value, out isChecked);
    }
}
