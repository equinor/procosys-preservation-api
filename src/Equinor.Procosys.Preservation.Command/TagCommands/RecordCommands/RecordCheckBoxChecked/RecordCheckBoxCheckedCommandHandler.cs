using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands.RecordCheckBoxChecked
{
    public class RecordCheckBoxCheckedCommandHandler : IRequestHandler<RecordCheckBoxCheckedCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RecordCheckBoxCheckedCommandHandler(
            IProjectRepository projectRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _requirementTypeRepository = requirementTypeRepository;
        }

        public async Task<Result<Unit>> Handle(RecordCheckBoxCheckedCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);

            var reqDef = await _requirementTypeRepository.GetRequirementDefinitionByFieldIdAsync(request.FieldId);
            var field = reqDef.Fields.Single(f => f.Id == request.FieldId);
            var requirement = tag.Requirements.Single(r => r.RequirementDefinitionId == reqDef.Id);

            var period = requirement.ActivePeriod;

            period.RemoveAnyOldFieldValue(request.FieldId);

            if (request.Value)
            {
                // save new value ONLY if CheckBox is Checked!
                period.AddFieldValue(new CheckBoxChecked(period.Schema, field));
            }

            period.UpdateStatus(reqDef);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
