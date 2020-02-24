using System;

namespace Equinor.Procosys.Preservation.Domain
{
    /// <summary>
    /// Interface to get and set creation and modification data on an entity.
    /// The methods are used by the context and should NOT be used by anyone else.
    /// </summary>
    public interface IAuditable
    {
        DateTime Created { get; }
        int CreatedById { get; }

        DateTime? Modified { get; }
        int? ModifiedById { get; }

        /// <summary>
        /// Method to set creation data on an entity.
        /// This is used by the context and should NOT be used by anyone else.
        /// </summary>
        /// <param name="creationDate">Creation date and time</param>
        /// <param name="createdById">Id of the user who created the entity</param>
        void SetCreated(DateTime creationDate, int createdById);

        /// <summary>
        /// Method to set modification data on an entity.
        /// This is used by the context and should NOT be used by anyone else.
        /// </summary>
        /// <param name="modifiedDate">Modification date and time</param>
        /// <param name="modifiedById">Id of the user who modified the entity</param>
        void SetModified(DateTime modifiedDate, int modifiedById);
    }
}
