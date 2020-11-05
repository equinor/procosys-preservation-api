using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    // todo validator .. see createareatagdto
    public class DuplicateAreaTagDto
    {
        public int TagId { get; set; }
        public AreaTagType AreaTagType { get; set; }
        public string DisciplineCode { get; set; }
        public string AreaCode { get; set; }
        public string PurchaseOrderCalloffCode { get; set; }
        public string TagNoSuffix { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string StorageArea { get; set; }
    }
}
