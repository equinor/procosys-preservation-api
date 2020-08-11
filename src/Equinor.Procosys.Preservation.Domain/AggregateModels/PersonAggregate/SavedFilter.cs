using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate
{
    public class SavedFilter : PlantEntityBase, ICreationAuditable
    {
        public const int TitleLengthMax = 255;
        public const int CriteriaLengthMax = 8000;

        public SavedFilter() : base(null)
        {
        }
        public SavedFilter(string plant, Project project, string title, string criteria)
            : base(plant)
        {
            if (project.Plant != plant)
            {
                throw new ArgumentException($"Can't relate item in {project.Plant} to item in {plant}");
            }

            ProjectId = project.Id;
            Title = title;
            Criteria = criteria;
        }

        public int ProjectId { get; }
        public string Title { get; }
        public string Criteria { get; }
        public bool DefaultFilter { get; set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }
    }
}
