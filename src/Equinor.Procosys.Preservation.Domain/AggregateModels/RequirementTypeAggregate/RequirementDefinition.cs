using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class RequirementDefinition : SchemaEntityBase
    {
        private readonly List<Field> _fields = new List<Field>();
        
        public const int TitleLengthMax = 64;

        protected RequirementDefinition()
            : base(null)
        {
        }

        public RequirementDefinition(string schema, string title, int defaultIntervalWeeks, int sortKey)
            : base(schema)
        {
            Title = title;
            DefaultIntervalWeeks = defaultIntervalWeeks;
            SortKey = sortKey;
        }

        public string Title { get; private set; }
        public bool IsVoided { get; private set; }
        public int DefaultIntervalWeeks { get; private set; }
        public int SortKey { get; private set; }
        public IReadOnlyCollection<Field> Fields => _fields.AsReadOnly();

        public void AddField(Field field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            _fields.Add(field);
        }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
    }
}
