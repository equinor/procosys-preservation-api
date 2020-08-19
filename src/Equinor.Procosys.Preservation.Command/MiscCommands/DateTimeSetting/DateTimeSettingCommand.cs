using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.MiscCommands.DateTimeSetting
{
    public class DateTimeSettingCommand : IRequest<Result<Unit>>
    {
        public DateTimeSettingCommand(string settingCode, DateTime value)
        {
            SettingCode = settingCode;
            Value = value;
        }

        public string SettingCode { get; }
        public DateTime Value { get; }
    }
}
