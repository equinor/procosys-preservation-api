namespace Equinor.Procosys.Preservation.Command.Validators
{
    public interface ITagValidator
    {
        bool Exists(int tagId);
        bool Exists(string tagNo, string projectNo);

        bool IsVoided(int tagId);
    }
}
