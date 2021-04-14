namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Persons
{
    public class UpdateSavedFilterDto
    {
        public string Title { get; set; }
        public string Criteria { get; set; }
        public bool? DefaultFilter { get; set; }
        public string RowVersion { get; set; }
    }
}
