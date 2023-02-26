namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public interface IRestrictionRolesChecker
    {
        bool HasCurrentUserExplicitNoRestrictions();
        bool HasCurrentUserExplicitAccessToContent(string responsibleCode);
    }
}
