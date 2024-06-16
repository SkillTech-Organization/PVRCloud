using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace FTLInsightsLogger.Logger
{
    public class TelemetryClientFactory
    {
        public static TelemetryLogger Create(string connectionString)
        {
            try
            {
                return new TelemetryLogger(connectionString);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
