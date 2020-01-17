﻿using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation
{
    public class StartPreservationCommand : IRequest<Result<Unit>>
    {
        public StartPreservationCommand(IEnumerable<int> tagIds)
            => TagIds = tagIds ?? throw new ArgumentNullException(nameof(tagIds));

        public IEnumerable<int> TagIds { get; }
    }
}
