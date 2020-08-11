using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetProjectByName;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Projects
{
    [ApiController]
    [Route("Projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator) => _mediator = mediator;

        [Authorize]
        [HttpGet("{projectName}")]
        public async Task<ActionResult<ProjectDetailsDto>> GetProject(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] string projectName)
        {
            var result = await _mediator.Send(new GetProjectByNameQuery(projectName));
            return this.FromResult(result);
        }
    }
}
