using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.MiscCommands.UpdateDateTimeSetting
{
    public class UpdateDateTimeSettingCommand : IRequest<Result<Unit>>
    {
        public UpdateDateTimeSettingCommand(string settingCode, DateTime value)
        {
            SettingCode = settingCode;
            Value = value;
        }

        public string SettingCode { get; }
        public DateTime Value { get; }
    }
}
