using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class RequirementValuesDto
    {
        public List<FieldValuesDto> FieldValues { get; set; }
        public string Comment { get; set; }
    }
}
