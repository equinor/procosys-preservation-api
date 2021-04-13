namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public interface IProjectAccessChecker
    {
        bool HasCurrentUserAccessToProject(string projectName);
    }
}
