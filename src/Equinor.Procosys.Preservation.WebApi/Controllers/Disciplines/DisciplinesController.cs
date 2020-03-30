using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetDisciplines;
using Equinor.Procosys.Preservation.WebApi.Misc;
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
        public async Task<ActionResult<List<DisciplineDto>>> GetDisciplines(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(Constants.Plant.MaxLength, MinimumLength = Constants.Plant.MinLength)]
            string plant)
        {
            var result = await _mediator.Send(new GetDisciplinesQuery(plant));
            return this.FromResult(result);
        }
    }
}
