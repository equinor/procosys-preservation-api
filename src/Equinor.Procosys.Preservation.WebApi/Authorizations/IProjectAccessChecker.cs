namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public interface IProjectAccessChecker
    {
        bool HasCurrentUserAccessToProject(string projectName);
    }
}
