namespace Equinor.Procosys.Preservation.Command.MainApi
{
    public class MainTagDto
    {
        public string TagNo { get; set; }
        public string Description { get; set; }

        public override string ToString() => TagNo;
    }
}
