namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class DuplicateAreaTagDto
    {
        public int SourceTagId { get; set; }
        public AreaTagType AreaTagType { get; set; }
        public string DisciplineCode { get; set; }
        public string AreaCode { get; set; }
        public string TagNoSuffix { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string StorageArea { get; set; }
    }
}
