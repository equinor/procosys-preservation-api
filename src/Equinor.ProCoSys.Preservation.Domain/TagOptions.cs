namespace Equinor.ProCoSys.Preservation.Domain
{
    public class TagOptions
    {
        public int IsNewHours { get; set; } = 48;
        public int TagSearchPageSize { get; set; } = 10000;
        public int MaxHistoryExport { get; set; } = 100;
    }
}
