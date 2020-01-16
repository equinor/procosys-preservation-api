namespace Equinor.Procosys.Preservation.Command.Validators.Responsible
{
    public interface IResponsibleValidator
    {
        bool Exists(int responsibleId);

        bool IsVoided(int responsibleId);
    }
}
