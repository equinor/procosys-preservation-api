namespace Equinor.Procosys.Preservation.Command.Validators.Project
{
    public interface IProjectValidator
    {
        bool Exists(string projectNo);
        bool IsClosed(string projectNo);
    }
}
