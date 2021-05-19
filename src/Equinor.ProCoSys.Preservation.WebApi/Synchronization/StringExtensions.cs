namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);
    }
}
