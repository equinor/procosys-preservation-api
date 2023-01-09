namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public enum PreservationStatus
    {
        // InService added as new Status early 2023.
        // Want to keep NotStarted as 0, but ordering tags with Status=InService before all other Statuses
        InService = -1,
        NotStarted,
        Active,
        Completed
    }
}
