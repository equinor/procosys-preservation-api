using System;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public class RowVersionValidator : IRowVersionValidator
    {
        public async Task<bool> IsValid(string rowVersion, CancellationToken cancellationToken) 
            => TryConvertBase64StringToByteArray(rowVersion, out _);

        private static bool TryConvertBase64StringToByteArray(string input, out byte[] output)
            {
                output = null;
                try
                {
                    output = Convert.FromBase64String(input);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
    }
}
