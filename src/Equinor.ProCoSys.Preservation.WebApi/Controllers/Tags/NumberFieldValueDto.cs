namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class NumberFieldValueDto
    {
        public int FieldId { get; set; }
        public double? Value { get; set; }
        public bool IsNA { get; set; }
    }
}
