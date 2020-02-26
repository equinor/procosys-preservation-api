using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Domain
{
    /// <summary>
    /// Interface to get and set creation and modification data on an entity.
    /// The methods are used by the context and should NOT be used by anyone else.
    /// </summary>
    public interface IAuditable
    {
        DateTime CreatedAtUtc { get; }
        int CreatedById { get; }

        DateTime? ModifiedAtUtc { get; }
        int? ModifiedById { get; }

        /// <summary>
        /// Method to set creation data on an entity.
        /// This is used by the context and should NOT be used by anyone else.
        /// </summary>
        /// <param name="createdAtUtc">Creation date and time</param>
        /// <param name="createdBy">The user who created the entity</param>
        void SetCreated(DateTime createdAtUtc, Person createdBy);

        /// <summary>
        /// Method to set modification data on an entity.
        /// This is used by the context and should NOT be used by anyone else.
        /// </summary>
        /// <param name="modifiedAtUtc">Modification date and time</param>
        /// <param name="modifiedBy">The user who modified the entity</param>
        void SetModified(DateTime modifiedAtUtc, Person modifiedBy);
    }
}
