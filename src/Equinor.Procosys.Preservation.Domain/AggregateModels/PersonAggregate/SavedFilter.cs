using System;
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
        public SavedFilter(string plant, int projectId, string title, string criteria)
            : base(plant)
        {
            ProjectId = projectId;
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
