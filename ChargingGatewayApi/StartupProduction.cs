using System;
using Microsoft.Extensions.Configuration;

namespace ChargingGatewayApi
{
    public class StartupProduction : Startup
    {
        public StartupProduction(IConfiguration config, IServiceProvider provider) : base(config)
        {
        }
    }
}