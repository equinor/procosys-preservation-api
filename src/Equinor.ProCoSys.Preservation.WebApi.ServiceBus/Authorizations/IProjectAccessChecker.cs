namespace Equinor.ProCoSys.Preservation.WebApi.ServiceBus.Authorizations
{
    public interface IProjectAccessChecker
    {
        bool HasCurrentUserAccessToProject(string projectName);
    }
}
