namespace Equinor.Procosys.Preservation.Command.Validators
{
    public interface IProjectValidator
    {
        bool Exists(string projectNo);
        bool IsClosed(string projectNo);
    }
}
