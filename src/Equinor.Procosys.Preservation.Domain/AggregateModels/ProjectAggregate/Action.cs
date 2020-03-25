using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Action : PlantEntityBase, ICreationAuditable, IModificationAuditable
    {
        public const int TitleLengthMax = 128;
        public const int DescriptionLengthMax = 4096;

        protected Action() : base(null)
        {
        }

        public Action(string plant, string title, string description, DateTime? dueTimeUtc)
            : base(plant)
        {
            Title = title;
            Description = description;
            SetDueTime(dueTimeUtc);
        }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime? DueTimeUtc { get; private set; }
        public DateTime? ClosedAtUtc { get; private set; }
        public int? ClosedById { get; private set; }
        public bool IsClosed => ClosedAtUtc.HasValue;
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void SetDueTime(DateTime? dueTimeUtc)
        {
            if (dueTimeUtc.HasValue && dueTimeUtc.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(dueTimeUtc)} is not Utc");
            }

            if (IsClosed)
            {
                throw new Exception($"Action {Id} is closed");
            }

            DueTimeUtc = dueTimeUtc;
        }

        public void Close(DateTime closedAtUtc, Person closedBy)
        {
            if (closedBy == null)
            {
                throw new ArgumentNullException(nameof(closedBy));
            }
            if (closedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(closedAtUtc)} is not Utc");
            }

            ClosedAtUtc = closedAtUtc;
            ClosedById = closedBy.Id;
        }

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;
        }
    }
}
