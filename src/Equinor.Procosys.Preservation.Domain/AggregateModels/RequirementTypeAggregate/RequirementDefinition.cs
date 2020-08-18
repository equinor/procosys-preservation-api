using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;
using Equinor.Procosys.Preservation.Domain.Time;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class RequirementDefinition : PlantEntityBase, ICreationAuditable, IModificationAuditable
    {
        private readonly List<Field> _fields = new List<Field>();
        
        public const int TitleLengthMax = 64;
        public const int UsageMax = 32; // must be at least length of longest RequirementUsage enum

        protected RequirementDefinition()
            : base(null)
        {
        }

        public RequirementDefinition(string plant, string title, int defaultIntervalWeeks, RequirementUsage usage, int sortKey)
            : base(plant)
        {
            Title = title;
            DefaultIntervalWeeks = defaultIntervalWeeks;
            Usage = usage;
            SortKey = sortKey;
        }

        public string Title { get; set; }
        public bool IsVoided { get; private set; }
        public int DefaultIntervalWeeks { get; set; }
        public RequirementUsage Usage { get; set; }
        public int SortKey { get; set; }
        public IReadOnlyCollection<Field> Fields => _fields.AsReadOnly();
        public bool NeedsUserInput => _fields.Any(f => f.NeedsUserInput);

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void AddField(Field field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            
            if (field.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {field.Plant} to item in {Plant}");
            }

            _fields.Add(field);
        }

        public IOrderedEnumerable<Field> OrderedFields(bool includeVoided)
            => Fields
                .Where(f => includeVoided || !f.IsVoided)
                .OrderBy(f => f.SortKey);

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public override string ToString() => $"{Title} - {Usage}";

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
