namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public static class FieldTypeExtension
    {
        public static bool NeedsUserInput(this FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Info:
                    return false;
                default:
                    return true;
            }
        }
    }
}
