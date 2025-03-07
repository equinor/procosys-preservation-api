namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Authorizations
{
    public interface IProjectAccessChecker
    {
        bool HasCurrentUserAccessToProject(string projectName);
    }
}
