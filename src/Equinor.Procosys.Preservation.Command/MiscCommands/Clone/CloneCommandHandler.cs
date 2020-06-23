using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.MiscCommands.Clone
{
    public class CloneCommandHandler : IRequestHandler<CloneCommand, Result<Unit>>
    {
        private readonly IPlantProvider _plantProvider;
        private readonly IPlantSetter _plantSetter;
        private readonly IModeRepository _modeRepository;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly ITagFunctionRepository _tagFunctionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CloneCommandHandler(
            IPlantProvider plantProvider,
            IPlantSetter plantSetter,
            IUnitOfWork unitOfWork,
            IModeRepository modeRepository,
            IResponsibleRepository responsibleRepository,
            IRequirementTypeRepository requirementTypeRepository,
            ITagFunctionRepository tagFunctionRepository)
        {
            _plantProvider = plantProvider;
            _plantSetter = plantSetter;
            _modeRepository = modeRepository;
            _responsibleRepository = responsibleRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _tagFunctionRepository = tagFunctionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(CloneCommand request, CancellationToken cancellationToken)
        {
            var targetPlant = _plantProvider.Plant;

            if (targetPlant != request.TargetPlant)
            {
                throw new Exception($"Target plant '{request.TargetPlant}' in request must match target plant '{targetPlant}' in provider");
            }

            await CloneModes(request.SourcePlant, targetPlant);
            await CloneResponsibles(request.SourcePlant, targetPlant);
            await CloneRequirementTypes(request.SourcePlant, targetPlant);
            
            // Need to save RequirementDefinitions to get let EF Core set ID's on new Items
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // TagFunctions must be cloned after RequirementTypes and RequirementDefinitions
            await CloneTagFunctions(request.SourcePlant, targetPlant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }

        private async Task CloneModes(string sourcePlant, string targetPlant)
        {
            var originalPlant = _plantProvider.Plant;
            _plantSetter.SetPlant(sourcePlant);
            var sourceModes = await _modeRepository.GetAllAsync();
            _plantSetter.SetPlant(originalPlant);

            var targetModes = await _modeRepository.GetAllAsync();

            foreach (var sourceMode in sourceModes)
            {
                if (targetModes.SingleOrDefault(t => t.Title == sourceMode.Title) == null)
                {
                    var targetMode = new Mode(targetPlant, sourceMode.Title, sourceMode.ForSupplier);
                    _modeRepository.Add(targetMode);
                }
            }
        }

        private async Task CloneResponsibles(string sourcePlant, string targetPlant)
        {
            var originalPlant = _plantProvider.Plant;
            _plantSetter.SetPlant(sourcePlant);
            var sourceResponsibles = await _responsibleRepository.GetAllAsync();
            _plantSetter.SetPlant(originalPlant);

            var targetResponsibles = await _responsibleRepository.GetAllAsync();

            foreach (var sourceResponsible in sourceResponsibles)
            {
                if (targetResponsibles.SingleOrDefault(t => t.Description == sourceResponsible.Description) == null)
                {
                    var targetResponsible = new Responsible(targetPlant, sourceResponsible.Code, sourceResponsible.Description);
                    _responsibleRepository.Add(targetResponsible);
                }
            }
        }

        private async Task CloneRequirementTypes(string sourcePlant, string targetPlant)
        {
            var originalPlant = _plantProvider.Plant;
            _plantSetter.SetPlant(sourcePlant);
            var sourceRTs = await _requirementTypeRepository.GetAllAsync();
            _plantSetter.SetPlant(originalPlant);

            var targetRTs = await _requirementTypeRepository.GetAllAsync();

            foreach (var sourceRT in sourceRTs)
            {
                var targetRT = targetRTs.SingleOrDefault(t => t.Title == sourceRT.Title);
                if (targetRT == null)
                {
                    targetRT = new RequirementType(targetPlant, sourceRT.Code, sourceRT.Title, sourceRT.SortKey);
                    _requirementTypeRepository.Add(targetRT);
                }

                CloneRequirementDefinitions(sourceRT, targetRT);
            }
        }

        private void CloneRequirementDefinitions(RequirementType sourceRT, RequirementType targetRT)
        {
            foreach (var sourceRD in sourceRT.RequirementDefinitions)
            {
                var targetRD = targetRT.RequirementDefinitions.SingleOrDefault(t => t.Title == sourceRD.Title);
                if (targetRD == null)
                {
                    targetRD = new RequirementDefinition(
                        targetRT.Plant,
                        sourceRD.Title,
                        sourceRD.DefaultIntervalWeeks, 
                        sourceRD.Usage,
                        sourceRD.SortKey); 
                    targetRT.AddRequirementDefinition(targetRD);
                }

                CloneFields(sourceRD, targetRD);
            }
        }

        private void CloneFields(RequirementDefinition sourceRD, RequirementDefinition targetRD)
        {
            foreach (var sourceField in sourceRD.Fields)
            {
                if (targetRD.Fields.SingleOrDefault(t => t.Label == sourceField.Label) == null)
                {
                    var targetField = new Field(
                        targetRD.Plant,
                        sourceField.Label,
                        sourceField.FieldType,
                        sourceField.SortKey,
                        sourceField.Unit,
                        sourceField.ShowPrevious);
                    targetRD.AddField(targetField);
                }
            }
        }

        private async Task CloneTagFunctions(string sourcePlant, string targetPlant)
        {
            var originalPlant = _plantProvider.Plant;
            _plantSetter.SetPlant(sourcePlant);
            var sourceTagFunctions = await _tagFunctionRepository.GetAllAsync();
            var sourceRTs = await _requirementTypeRepository.GetAllAsync();
            var sourceRDs = sourceRTs.SelectMany(rt => rt.RequirementDefinitions).ToList();
            _plantSetter.SetPlant(originalPlant);

            var targetTagFunctions = await _tagFunctionRepository.GetAllAsync();
            var targetRTs = await _requirementTypeRepository.GetAllAsync();
            var targetRDs = targetRTs.SelectMany(rt => rt.RequirementDefinitions).ToList();

            foreach (var sourceTagFunction in sourceTagFunctions)
            {
                var targetTagFunction = targetTagFunctions.SingleOrDefault(
                    t => t.Code == sourceTagFunction.Code && t.RegisterCode == sourceTagFunction.RegisterCode);
                if (targetTagFunction == null)
                {
                    targetTagFunction = new TagFunction(
                        targetPlant,
                        sourceTagFunction.Code,
                        sourceTagFunction.Description,
                        sourceTagFunction.RegisterCode);
                    _tagFunctionRepository.Add(targetTagFunction);
                }

                CloneRequirements(sourceRDs, sourceTagFunction, targetRDs, targetTagFunction);
            }
        }

        private void CloneRequirements(
            List<RequirementDefinition> sourceRDs,
            TagFunction sourceTagFunction,
            List<RequirementDefinition> targetRDs,
            TagFunction targetTagFunction)
        {
            foreach (var sourceRequirement in sourceTagFunction.Requirements)
            {
                var sourceRD = sourceRDs.Single(s => s.Id == sourceRequirement.RequirementDefinitionId);
                var targetRD = targetRDs.Single(t => t.Title == sourceRD.Title);
                if (targetTagFunction.Requirements.SingleOrDefault(r => r.RequirementDefinitionId == targetRD.Id) == null)
                {
                    var targetRequirement = new TagFunctionRequirement(targetTagFunction.Plant, sourceRequirement.IntervalWeeks, targetRD);
                    targetTagFunction.AddRequirement(targetRequirement);
                }
            }
        }
    }
}
