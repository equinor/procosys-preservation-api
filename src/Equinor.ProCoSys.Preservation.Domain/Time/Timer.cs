using System.Diagnostics;

namespace Equinor.ProCoSys.Preservation.Domain.Time
{
    public class Timer
    {
        private Stopwatch _stopWatch;
        private long _previousElapsedFromStart = 0;

        public Timer() => _stopWatch = Stopwatch.StartNew();

        public string Elapsed()
        {
            long elapsedFromStart = _stopWatch.ElapsedMilliseconds;

            var elapsed = $"{elapsedFromStart - _previousElapsedFromStart}ms / {elapsedFromStart}ms";
            _previousElapsedFromStart = elapsedFromStart;
            return elapsed;
        }
    }
}
