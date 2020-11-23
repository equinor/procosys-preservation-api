using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class RequirementValuesDto
    {
        public List<NumberFieldValueDto> NumberValues { get; set; } = new List<NumberFieldValueDto>();
        public List<CheckBoxFieldValueDto> CheckBoxValues { get; set; } = new List<CheckBoxFieldValueDto>();
        public string Comment { get; set; }
    }
}
