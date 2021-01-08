using ClosedXML.Excel;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public class XLFile
    {
        public XLWorkbook Workbook { get; set; }
        public string ContentType { get; set; }
    }
}
