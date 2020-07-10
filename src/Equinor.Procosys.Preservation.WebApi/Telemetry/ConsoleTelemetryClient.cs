using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Equinor.Procosys.Preservation.WebApi.Telemetry
{
    public class ConsoleTelemetryClient : ITelemetryClient
    {
        public void TrackEvent(string name, Dictionary<string, string> properties)
        {
            var builder = new StringBuilder();
            builder.Append($"Event:\t{name}:");

            if (properties != null && properties.Any())
            {
                foreach (var property in properties)
                {
                    builder.Append(Environment.NewLine);
                    builder.Append($"\t{property.Key}: {property.Value}");
                }
            }

            Console.WriteLine(builder.ToString());
        }

        public void TrackMetric(string name, double metric) =>
            Console.WriteLine($"Metric:\t{name}:{Environment.NewLine}\t{metric}");

        public void TrackMetric(string name, double metric, string dimension1Name, string dimension1Value) =>
            Console.WriteLine($"Metric:\t{name}:{Environment.NewLine}\t{metric}{Environment.NewLine}\t{dimension1Name}: {dimension1Value}");

        public void TrackMetric(string name, double metric, string dimension1Name, string dimension2Name, string dimension1Value, string dimension2Value) =>
            Console.WriteLine($"Metric:\t{name}:{Environment.NewLine}\t{metric}{Environment.NewLine}\t{dimension1Name}: {dimension1Value}{Environment.NewLine}\t\t{dimension2Name}: {dimension2Value}");
    }
}
