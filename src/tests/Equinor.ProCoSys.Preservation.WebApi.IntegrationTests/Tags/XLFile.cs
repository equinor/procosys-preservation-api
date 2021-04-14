using ClosedXML.Excel;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class XLFile
    {
        public XLWorkbook Workbook { get; set; }
        public string ContentType { get; set; }
    }
}
