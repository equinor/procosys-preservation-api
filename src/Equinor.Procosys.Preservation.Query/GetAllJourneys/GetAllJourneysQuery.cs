﻿using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetAllJourneys
{
    public class GetAllJourneysQuery : IRequest<Result<IEnumerable<JourneyDto>>>
    {
        public GetAllJourneysQuery(bool includeVoided) => IncludeVoided = includeVoided;

        public bool IncludeVoided { get; }
    }
}