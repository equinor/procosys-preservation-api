using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public class TagDetailsDto
    {
        public int Id { get; set; }
        public string TagNo { get; set; }
        public string AreaCode { get; set; }
        public string DisciplineCode { get; set; }
        public TagType TagType { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public JourneyDetailsDto Journey { get; set; }
        public StepDetailsDto Step { get; set; }
        public ModeDetailsDto Mode { get; set; }
        public ResponsibleDetailsDto Responsible { get; set; }
        public string RowVersion { get; set; }
    }
}
