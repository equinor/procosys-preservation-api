using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;
using Equinor.Procosys.Preservation.Domain.Time;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Project : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable
    {
        private readonly List<Tag> _tags = new List<Tag>();
        
        public const int NameLengthMax = 30;
        public const int DescriptionLengthMax = 1000;

        protected Project()
            : base(null)
        {
        }

        public Project(string plant, string name, string description)
            : base(plant)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; set; }
        public bool IsClosed { get; set; }
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void AddTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (tag.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {tag.Plant} to item in {Plant}");
            }

            _tags.Add(tag);
        }

        public void Close() => IsClosed = true;

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

        // todo add unit tests. See RemoveStep
        public void RemoveTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }
            
            if (!tag.IsVoided)
            {
                throw new Exception($"{nameof(tag)} must be voided before delete");
            }

            if (tag.Plant != Plant)
            {
                throw new ArgumentException($"Can't remove item in {tag.Plant} from item in {Plant}");
            }

            _tags.Remove(tag);
        }
    }
}
