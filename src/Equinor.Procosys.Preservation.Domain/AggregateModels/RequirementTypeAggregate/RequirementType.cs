using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class RequirementType : SchemaEntityBase, IAggregateRoot
    {
        private readonly List<RequirementDefinition> _requirementDefinitions = new List<RequirementDefinition>();

        public const int CodeLengthMax = 32;
        public const int TitleLengthMax = 64;

        protected RequirementType()
            : base(null)
        {
        }

        public RequirementType(string schema, string code, string title, int sortKey)
            : base(schema)
        {
            Code = code;
            Title = title;
            SortKey = sortKey;
        }

        public string Code { get; private set; }
        public string Title { get; private set; }
        public bool IsVoided { get; private set; }
        public int SortKey { get; private set; }
        public IReadOnlyCollection<RequirementDefinition> RequirementDefinitions => _requirementDefinitions.AsReadOnly();

        public void AddRequirementDefinition(RequirementDefinition requirementDefinition)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }

            _requirementDefinitions.Add(requirementDefinition);
        }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
    }
}
