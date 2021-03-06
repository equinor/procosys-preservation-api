﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate
{
    public interface ITagFunctionRepository : IRepository<TagFunction>
    {
        Task<TagFunction> GetByCodesAsync(string tagFunctionCode, string registerCode);
        Task<List<TagFunction>> GetAllNonVoidedWithRequirementsAsync();
    }
}
