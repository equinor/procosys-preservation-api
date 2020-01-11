namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class ProjectValidator : IProjectValidator
    {
        public bool Exists(string projectNo) => true;

        public bool IsClosed(string projectNo) => false;
    }
}
