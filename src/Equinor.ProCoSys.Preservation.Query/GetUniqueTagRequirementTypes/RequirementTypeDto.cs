﻿using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagRequirementTypes
{
    public class RequirementTypeDto
    {
        public RequirementTypeDto(int id, string code, RequirementTypeIcon icon, string title)
        {
            Id = id;
            Code = code;
            Icon = icon;
            Title = title;
        }

        public int Id { get; }
        public string Code { get; }
        public RequirementTypeIcon Icon { get; }
        public string Title { get; }
    }
}
