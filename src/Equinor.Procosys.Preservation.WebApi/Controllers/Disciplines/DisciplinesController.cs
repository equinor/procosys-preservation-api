using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetDisciplines;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Disciplines
{
    [ApiController]
    [Route("Disciplines")]
    public class DisciplinesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DisciplinesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<DisciplineDto>>> GetDisciplines()
        {
            var result = await _mediator.Send(new GetDisciplinesQuery());
            return this.FromResult(result);
        }
    }
}
