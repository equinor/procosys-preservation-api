namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public interface IContentRestrictionsChecker
    {
        bool HasCurrentUserExplicitNoRestrictions();
        bool HasCurrentUserExplicitAccessToContent(string responsibleCode);
    }
}
