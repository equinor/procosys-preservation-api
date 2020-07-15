namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class UpdateFieldDto : FieldDto
    {
        public int Id { get; set; }
        public string RowVersion { get; set; }
    }
}
