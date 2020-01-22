namespace Equinor.Procosys.Preservation.Command.Validators.Project
{
    public interface IProjectValidator
    {
        bool Exists(string projectName);
        bool IsClosed(string projectName);
    }
}
