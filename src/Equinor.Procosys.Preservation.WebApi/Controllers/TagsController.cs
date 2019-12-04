﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands;
using Equinor.Procosys.Preservation.Query.TagAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    [ApiController]
    [Route("Tags")]
    public class TagsController : ControllerBase
    {
        private readonly ILogger<TagsController> _logger;
        private readonly IMediator _mediator;

        public TagsController(ILogger<TagsController> logger, IMediator mediator)
        {
            _logger = logger;
            this._mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
        {
            IEnumerable<TagDto> tags = await _mediator.Send(new AllTagsQuery());
            return Ok(tags);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDto dto)
        {
            int tagId = await _mediator.Send(new CreateTagCommand(dto.TagNo, dto.ProjectNo, dto.Schema));
            return Ok(tagId);
        }
    }
}
