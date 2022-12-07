using System;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.SettingAggregate
{
    public class Setting : EntityBase, IAggregateRoot
    {
        public const int CodeLengthMax = 64;

        public string Code { get; set; }
        public string Value { get; set; }
    }
}
