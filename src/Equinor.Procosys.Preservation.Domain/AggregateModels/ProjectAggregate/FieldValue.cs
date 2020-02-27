﻿using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public abstract class FieldValue : SchemaEntityBase, ICreationAuditable
    {
        protected FieldValue()
            : base(null)
        {
        }

        protected FieldValue(string schema, Field field)
            : base(schema)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            FieldId = field.Id;
        }
        
        public int FieldId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }

        public void SetCreated(DateTime createdAtUtc, Person createdBy)
        {
            if (createdAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(createdAtUtc)} is not UTC");
            }

            CreatedAtUtc = createdAtUtc;
            CreatedById = createdBy.Id;
        }
    }
}
