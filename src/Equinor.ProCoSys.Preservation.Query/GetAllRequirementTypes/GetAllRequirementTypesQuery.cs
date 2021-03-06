﻿using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetAllRequirementTypes
{
    public class GetAllRequirementTypesQuery : IRequest<Result<IEnumerable<RequirementTypeDto>>>
    {
        public GetAllRequirementTypesQuery(bool includeVoided) => IncludeVoided = includeVoided;

        public bool IncludeVoided { get; }
    }
}
