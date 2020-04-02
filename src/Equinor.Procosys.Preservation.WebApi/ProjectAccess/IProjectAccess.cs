namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public interface IProjectAccess
    {
        ProjectAccessFailure ValidateAccess(object request);
    }
}
