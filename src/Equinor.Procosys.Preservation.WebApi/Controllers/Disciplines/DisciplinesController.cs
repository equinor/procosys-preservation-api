using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetDisciplines;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = Permissions.LIBRARY_GENERAL_READ)]
        [HttpGet]
        public async Task<ActionResult<List<DisciplineDto>>> GetDisciplines(
            [FromHeader( Name = PlantProvider.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var result = await _mediator.Send(new GetDisciplinesQuery());
            return this.FromResult(result);
        }
    }
}
