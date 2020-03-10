using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues;
using Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Query.GetTagActionDetails;
using Equinor.Procosys.Preservation.Query.GetTagActions;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Equinor.Procosys.Preservation.Query.GetTagRequirements;
using Equinor.Procosys.Preservation.Query.GetTags;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;
using GetAllTagsInProjectQuery = Equinor.Procosys.Preservation.Query.ProjectAggregate.GetAllTagsInProjectQuery;
using Requirement = Equinor.Procosys.Preservation.Command.TagCommands.Requirement;
using RequirementDto = Equinor.Procosys.Preservation.Query.GetTagRequirements.RequirementDto;
using RequirementPreserveCommand = Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve.PreserveCommand;
using TagDto = Equinor.Procosys.Preservation.Query.ProjectAggregate.TagDto;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    /// <summary>
    /// Handles requests that deal with preservation tags
    /// </summary>
    [ApiController]
    [Route("Tags")]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTagsInProject([FromQuery] string projectName)    
        {
            var result = await _mediator.Send(new GetAllTagsInProjectQuery(projectName));
            return this.FromResult(result);
        }
        
        [HttpGet("Filtered")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags(
            [FromQuery] string projectName,
            [FromQuery] PreservationStatus? preservationStatus,
            [FromQuery] IEnumerable<int> requirementTypeIds,
            [FromQuery] IEnumerable<DueFilterType> dueFilters,
            [FromQuery] IEnumerable<int> responsibleIds,
            [FromQuery] IEnumerable<int> disciplineIds,
            [FromQuery] IEnumerable<string> tagFunctionCodes,
            [FromQuery] IEnumerable<int> modeIds,
            [FromQuery] IEnumerable<int> journeyIds,
            [FromQuery] IEnumerable<int> stepIds,
            [FromQuery] string tagNoStartsWith,
            [FromQuery] string commPkgNoStartsWith,
            [FromQuery] string mcPkgNoStartsWith,
            [FromQuery] string purchaseOrderNoStartsWith,
            [FromQuery] string callOffStartsWith,
            SortingColumn sortingColumn = SortingColumn.Due,
            SortingDirection sortingDirection = SortingDirection.Asc,
            int page = 0,
            int pageSize = 20
        )
        {
            // todo validation of project page and pagesize
            var query = new GetTagsQuery(
                new Sorting(sortingDirection, sortingColumn),
                new Filter(
                    projectName,
                    dueFilters,
                    preservationStatus,
                    requirementTypeIds,
                    disciplineIds,
                    responsibleIds,
                    tagFunctionCodes,
                    modeIds,
                    journeyIds,
                    stepIds,
                    tagNoStartsWith,
                    commPkgNoStartsWith,
                    mcPkgNoStartsWith,
                    purchaseOrderNoStartsWith,
                    callOffStartsWith),
                new Paging(page, pageSize)
            );
            var result = await _mediator.Send(query);
            return this.FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDetailsDto>> GetTagDetails([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagDetailsQuery(id));
            return this.FromResult(result);
        }

        [HttpGet("{id}/Requirements")]
        public async Task<ActionResult<List<RequirementDto>>> GetTagRequirements([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagRequirementsQuery(id));
            return this.FromResult(result);
        }

        [HttpGet("{id}/Actions")]
        public async Task<ActionResult<List<ActionDto>>> GetTagActions([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetTagActionsQuery(id));
            return this.FromResult(result);
        }

        [HttpGet("{id}/Actions/{actionId}")]
        public async Task<ActionResult<ActionDetailsDto>> GetTagActionDetails([FromRoute] int id, [FromRoute] int actionId)
        {
            var result = await _mediator.Send(new GetActionDetailsQuery(id, actionId));
            return this.FromResult(result);
        }

        [HttpPost("Standard")]
        public async Task<ActionResult<int>> CreateTag([FromBody] CreateTagDto dto)
        {
            var requirements = dto.Requirements?
                .Select(r =>
                    new Requirement(r.RequirementDefinitionId, r.IntervalWeeks));
            var result = await _mediator.Send(
                new CreateTagCommand(
                    dto.TagNos,
                    dto.ProjectName,
                    dto.StepId,
                    requirements,
                    dto.Remark));
            return this.FromResult(result);
        }

        [HttpPost("Area")]
        public async Task<ActionResult<int>> CreateAreaTag([FromBody] CreateAreaTagDto dto)
        {
            var requirements = dto.Requirements?
                .Select(r =>
                    new Requirement(r.RequirementDefinitionId, r.IntervalWeeks));
            
            var result = await _mediator.Send(
                new CreateAreaTagCommand(
                    dto.ProjectName,
                    dto.AreaTagType.ConvertToTagType(),
                    dto.DisciplineCode,
                    dto.AreaCode,
                    dto.TagNoSuffix,
                    dto.StepId,
                    requirements,
                    dto.Description,
                    dto.Remark));

            return this.FromResult(result);
        }

        // todo remove handling of SetStepCommand if still not used in medio 2020
        //[HttpPut("{id}/SetStep")]
        //public async Task<ActionResult> SetStep([FromRoute] int id, [FromBody] SetStepDto dto)
        //{
        //    var result = await _mediator.Send(new SetStepCommand(id, dto.StepId));
        //    return this.FromResult(result);
        //}

        [HttpPut("{id}/StartPreservation")]
        public async Task<IActionResult> StartPreservation([FromRoute] int id)
        {
            var result = await _mediator.Send(new StartPreservationCommand(new List<int>{id}));
            return this.FromResult(result);
        }

        [HttpPut("StartPreservation")]
        public async Task<IActionResult> StartPreservation([FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new StartPreservationCommand(tagIds));
            return this.FromResult(result);
        }

        [HttpPut("{id}/Preserve")]
        public async Task<IActionResult> Preserve([FromRoute] int id)
        {
            var result = await _mediator.Send(new PreserveCommand(id));
            return this.FromResult(result);
        }

        [HttpPut("BulkPreserve")]
        public async Task<IActionResult> BulkPreserve([FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new BulkPreserveCommand(tagIds));
            return this.FromResult(result);
        }

        [HttpPut("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new TransferCommand(tagIds));
            return this.FromResult(result);
        }

        [HttpPost("{id}/Requirement/{requirementId}/RecordValues")]
        public async Task<IActionResult> RecordCheckBoxChecked([FromRoute] int id, [FromRoute] int requirementId, [FromBody] RequirementValuesDto requirementValuesDto)
        {
            var fieldValues = requirementValuesDto?
                .FieldValues
                .ToDictionary(
                    keySelector => keySelector.FieldId,
                    elementSelector => elementSelector.Value);

            var result = await _mediator.Send(new RecordValuesCommand(id, requirementId, fieldValues, requirementValuesDto?.Comment));
            
            return this.FromResult(result);
        }

        [HttpPost("{id}/Requirement/{requirementId}/Preserve")]
        public async Task<IActionResult> Preserve([FromRoute] int id, [FromRoute] int requirementId)
        {
            var result = await _mediator.Send(new RequirementPreserveCommand(id, requirementId));
            
            return this.FromResult(result);
        }
    }
}
