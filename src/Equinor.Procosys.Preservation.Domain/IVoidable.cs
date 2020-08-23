namespace Equinor.Procosys.Preservation.Domain
{
    public interface IVoidable
    {
        bool IsVoided { get; set; }
    }
}
