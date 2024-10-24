﻿using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public interface IPlantEntityEvent<out T> where T : PlantEntityBase, IHaveGuid
{
    T Entity { get; }
}
