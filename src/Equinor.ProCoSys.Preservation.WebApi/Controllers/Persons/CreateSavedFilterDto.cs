namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Persons
{
    public class CreateSavedFilterDto
    {
        public string ProjectName { get; set; }
        public string Title { get; set; }
        public string Criteria { get; set; }
        public bool DefaultFilter { get; set; }
    }
}
