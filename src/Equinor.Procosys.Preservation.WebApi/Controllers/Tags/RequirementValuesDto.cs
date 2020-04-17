using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class RequirementValuesDto
    {
        public List<NumberFieldValueDto> NumberValues { get; set; }
        public List<CheckBoxFieldValueDto> CheckBoxValues { get; set; }
        public string Comment { get; set; }
    }
}
