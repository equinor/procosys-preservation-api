using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Responsible
{
    public class ResponsibleValidator : IResponsibleValidator
    {
        private readonly IResponsibleRepository _responsibleRepository;

        public ResponsibleValidator(IResponsibleRepository responsibleRepository)
            => _responsibleRepository = responsibleRepository;

        public bool Exists(int responsibleId)
            => _responsibleRepository.GetByIdAsync(responsibleId).Result != null;

        public bool IsVoided(int responsibleId) => throw new System.NotImplementedException("responsible.isvoided"); // todo
    }
}
