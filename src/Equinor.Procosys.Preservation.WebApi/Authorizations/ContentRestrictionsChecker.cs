namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class ContentRestrictionsChecker : IContentRestrictionsChecker
    {
        public bool HasCurrentUserAnyRestrictions()
        {
            return false;
        }

        public bool HasCurrentUserExplicitAccessToContent(string responsibleCode)
        {
            return true;
        }
    }
}
