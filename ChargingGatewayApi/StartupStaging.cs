using Microsoft.Extensions.Configuration;

namespace ChargingGatewayApi
{
    public class StartupStaging : Startup
    {
        public StartupStaging(IConfiguration config) : base(config)
        {
        }
    }
}