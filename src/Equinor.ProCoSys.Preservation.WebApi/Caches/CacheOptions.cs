namespace Equinor.ProCoSys.Preservation.WebApi.Caches
{
    public class CacheOptions
    {
        public int PersonCacheMinutes { get; set; } = 1440;
        public int PermissionCacheMinutes { get; set; } = 20;
        public int PlantCacheMinutes { get; set; } = 20;
    }
}
