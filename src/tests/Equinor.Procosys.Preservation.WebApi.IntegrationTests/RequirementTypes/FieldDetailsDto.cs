namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    public class FieldDetailsDto : FieldDto
    {
        public int Id { get; set; }
        public bool IsVoided { get; set; }
        public string RowVersion { get; set; }
    }
}
