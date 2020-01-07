namespace Equinor.Procosys.Preservation.MainApi
{
    public class ProcosysPlant
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public override string ToString() => $"{Title} ({Id})";
    }
}
