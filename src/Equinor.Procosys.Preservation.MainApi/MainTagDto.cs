namespace Equinor.Procosys.Preservation.MainApi
{
    public class MainTagDto
    {
        public string TagNo { get; set; }
        public string Description { get; set; }

        public override string ToString() => TagNo;
    }
}
