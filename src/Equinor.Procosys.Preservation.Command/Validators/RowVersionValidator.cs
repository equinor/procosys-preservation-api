using System;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class RowVersionValidator : IRowVersionValidator
    {
        public async Task<bool> IsValid(string rowVersion, CancellationToken cancellationToken)
            => !string.IsNullOrWhiteSpace(rowVersion) && TryConvertBase64StringToByteArray(rowVersion);

        private static bool TryConvertBase64StringToByteArray(string input)
        {
            try
            {
                Convert.FromBase64String(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
