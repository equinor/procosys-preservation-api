﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate
{
    public class Person : EntityBase, IAggregateRoot, IModificationAuditable, IHaveGuid
    {
        public const int FirstNameLengthMax = 64;
        public const int LastNameLengthMax = 64;

        private readonly List<SavedFilter> _savedFilters = new();

        protected Person() : base()
        {
        }

        public Person(Guid oid, string firstName, string lastName) : base()
        {
            Guid = oid;
            FirstName = firstName;
            LastName = lastName;
        }

        // private setters needed for Entity Framework
        public IReadOnlyCollection<SavedFilter> SavedFilters => _savedFilters.AsReadOnly();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }
        public Guid Guid { get; private set; }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;
        }

        public void AddSavedFilter(SavedFilter savedFilter)
        {
            if (savedFilter == null)
            {
                throw new ArgumentNullException(nameof(savedFilter));
            }

            _savedFilters.Add(savedFilter);
        }

        public void RemoveSavedFilter(SavedFilter savedFilter)
        {
            if (savedFilter == null)
            {
                throw new ArgumentNullException(nameof(savedFilter));
            }

            _savedFilters.Remove(savedFilter);
        }

        public SavedFilter GetDefaultFilter(int projectId) =>
            _savedFilters.SingleOrDefault(s => s.ProjectId == projectId && s.DefaultFilter);
    }
}
