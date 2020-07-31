using System;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate
{
    public class SavedFilter : PlantEntityBase, ICreationAuditable
    {
        public const int TitleLengthMax = 255;
        public const int CriteriaLengthMax = 1024;

        public SavedFilter(string plant) : base(plant)
        {
        }
        public SavedFilter(string plant, string title, string criteria)
            : base(plant)
        {
            Title = title;
            Criteria = criteria;
        }
        public string Title { get; }
        public string Criteria { get; }
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
