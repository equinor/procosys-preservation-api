using System;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetDateTimeSetting
{
    public class GetDateTimeSettingQuery : IRequest<Result<DateTime?>>
    {
        public GetDateTimeSettingQuery(string settingCode) => SettingCode = settingCode;

        public string SettingCode { get; }
    }
}
