using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class CreateAreaTagDto
    {
        public string ProjectName { get; set; }
        public AreaTagType AreaTagType{ get; set; }
        public string DisciplineCode{ get; set; }
        public string AreaCode{ get; set; }
        public string TagNoSuffix{ get; set; }
        public int StepId { get; set; }
        public IEnumerable<TagRequirementDto> Requirements { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string StorageArea { get; set; }
    }
}
