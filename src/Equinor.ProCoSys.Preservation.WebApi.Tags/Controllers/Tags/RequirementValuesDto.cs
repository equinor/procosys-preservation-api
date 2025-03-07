using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags
{
    public class RequirementValuesDto
    {
        public List<NumberFieldValueDto> NumberValues { get; set; } = new List<NumberFieldValueDto>();
        public List<CheckBoxFieldValueDto> CheckBoxValues { get; set; } = new List<CheckBoxFieldValueDto>();
        public string Comment { get; set; }
    }
}
