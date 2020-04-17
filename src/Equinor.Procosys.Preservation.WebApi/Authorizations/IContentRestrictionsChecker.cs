namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public interface IContentRestrictionsChecker
    {
        bool HasCurrentUserAnyRestrictions();
        bool HasCurrentUserExplicitAccessToContent(string responsibleCode);
    }
}
