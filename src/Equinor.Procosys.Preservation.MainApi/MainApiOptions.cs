namespace Equinor.ProCoSys.Preservation.MainApi
{
    public class MainApiOptions
    {
        public string ApiVersion { get; set; }
        public string BaseAddress { get; set; }
        public int TagSearchPageSize { get; set; } = 100;
    }
}
