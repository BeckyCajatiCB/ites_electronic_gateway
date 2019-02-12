using System;
using System.Net.Http;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChargingGatewayApi.Tests
{
    public class StartupTest : Startup
    {
        public HttpMessageHandler IdpHandler;

        public StartupTest(IConfiguration config) : base(config)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(IdentityProviderSeedData.GetApiResourceses())
                .AddInMemoryClients(IdentityProviderSeedData.GetClients());

            services.AddSingleton(this);

            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            app.UseIdentityServer();
            base.Configure(app, env, loggerFactory, serviceProvider);
        }


        public override void BindIdentityServerAuthenticationOptions(IdentityServerAuthenticationOptions options)
        {
            base.BindIdentityServerAuthenticationOptions(options);

            options.JwtBackChannelHandler = IdpHandler;
            options.IntrospectionBackChannelHandler = IdpHandler;
            options.IntrospectionDiscoveryHandler = IdpHandler;
        }
    }
}