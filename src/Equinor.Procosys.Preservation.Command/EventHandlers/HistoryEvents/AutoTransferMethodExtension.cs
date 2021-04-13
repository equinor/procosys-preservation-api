using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public static class AutoTransferMethodExtension
    {
        public static string CovertToString(this AutoTransferMethod autoTransferMethod)
            => autoTransferMethod switch
            {
                AutoTransferMethod.OnRfccSign => "On RFCC signing",
                AutoTransferMethod.OnRfocSign => "On RFOC signing",
                _ => autoTransferMethod.ToString()
            };
    }
}
