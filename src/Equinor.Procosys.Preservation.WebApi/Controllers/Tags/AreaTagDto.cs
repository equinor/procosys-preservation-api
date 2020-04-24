namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class AreaTagDto
    {
        public string ProjectName { get; set; }
        public AreaTagType AreaTagType { get; set; }
        public string DisciplineCode{ get; set; }
        public string AreaCode{ get; set; }
        public string TagNoSuffix{ get; set; }
    }
}
