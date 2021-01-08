namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public static class PurchaseOrderHelper
    {
        public static string CreateTitle(string purchaseOrderNo, string calloffNo)
            => string.IsNullOrEmpty(calloffNo) ? purchaseOrderNo : $"{purchaseOrderNo}/{calloffNo}";
    }
}
