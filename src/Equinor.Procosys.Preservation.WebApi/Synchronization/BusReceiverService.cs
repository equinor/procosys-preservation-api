using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.WebApi.Telemetry;

namespace Equinor.ProCoSys.IPO.WebApi.Synchronization
{
    public class BusReceiverService : IBusReceiverService
    {
        private readonly IPlantSetter _plantSetter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelemetryClient _telemetryClient;
        private readonly IResponsibleRepository _responsibleRepository;
        private const string IpoBusReceiverTelemetryEvent = "Preservation Bus Receiver";

        public BusReceiverService(
            IPlantSetter plantSetter,
            IUnitOfWork unitOfWork,
            ITelemetryClient telemetryClient,
            IResponsibleRepository responsibleRepository)
        {
            _plantSetter = plantSetter;
            _unitOfWork = unitOfWork;
            _telemetryClient = telemetryClient;
            _responsibleRepository = responsibleRepository;
        }

        public async Task ProcessMessageAsync(PcsTopic pcsTopic, string messageJson, CancellationToken cancellationToken)
        {
            switch (pcsTopic)
            {
                case PcsTopic.Responsible:
                    await ProcessResponsibleEvent(messageJson);
                    break;
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessResponsibleEvent(string messageJson)
        {
            var responsibleEvent = JsonSerializer.Deserialize<ResponsibleTopic>(messageJson);
            if (string.IsNullOrWhiteSpace(responsibleEvent.Plant) || string.IsNullOrWhiteSpace(responsibleEvent.Code))
            {
                throw new Exception($"Unable to deserialize JSON to ResponsibleEvent {messageJson}");
            }

            _telemetryClient.TrackEvent(IpoBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {PcsServiceBusTelemetryConstants.Event, ResponsibleTopic.TopicName},
                    {PcsServiceBusTelemetryConstants.ResponsibleCode, responsibleEvent.Code},
                    {PcsServiceBusTelemetryConstants.Plant, responsibleEvent.Plant[4..]},
                });
            _plantSetter.SetPlant(responsibleEvent.Plant);
            var responsible = await _responsibleRepository.GetByCodeAsync(responsibleEvent.Code);
            if (responsible != null)
            {
                responsible.Description = responsibleEvent.Description;
            }
        }
    }
}
