namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries
{
    public static class VoidedFilterTypeExtension
    {
        public static string GetDisplayValue(this VoidedFilterType voidedFilterType)
            => voidedFilterType switch
            {
                VoidedFilterType.All=> "All",
                VoidedFilterType.NotVoided => "Not voided",
                VoidedFilterType.Voided => "Voided",
                _ => string.Empty
            };
    }
}
