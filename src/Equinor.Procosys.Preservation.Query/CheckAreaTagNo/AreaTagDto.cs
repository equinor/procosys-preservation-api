namespace Equinor.ProCoSys.Preservation.Query.CheckAreaTagNo
{
    public class AreaTagDto
    {
        public AreaTagDto(string tagNo) => TagNo = tagNo;

        public string TagNo { get; }
        public bool Exists { get; set; }
    }
}
