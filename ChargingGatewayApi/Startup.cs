using System;
using App.Metrics.Health;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using LogApiRequest.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using SnakeCaseValueProviderFactory.Core;
using SwaggerFilters.Core;
using Swashbuckle.AspNetCore.Swagger;

namespace ChargingGatewayApi
{
    public abstract class Startup
    {
        protected Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            //var dbContext = services.BuildServiceProvider().GetService<SampleDbContext>();

            AppMetricsHealth.CreateDefaultBuilder()
            //    .HealthChecks.AddCheck(new DatabaseHealthCheck(dbContext))
                .BuildAndAddTo(services);

            services.AddMvc(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                    config.ValueProviderFactories.Add(new SnakeCaseQueryStringValueProviderFactory());
                })
                .AddJsonOptions(json =>
                {
                    json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    json.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy(true, false)
                    };
                });

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(BindIdentityServerAuthenticationOptions);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("docs", new Info {Title = "Template Api", Version = "v1"});
                c.OperationFilter<SnakeCaseFilter>();
                c.AddSecurityDefinition("oauth2",
                    new OAuth2Scheme
                    {
                        Description = "Requests an authorization token from Identity Provider",
                        TokenUrl = Configuration["IdentityProvider:Authority"] + "/connect/token",
                        Flow = "application"
                    });
                c.OperationFilter<OAuthFilter>();
            });

            services.AddAutoMapper();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            app.UseHealthAllEndpoints();

            app.UseAuthentication();

            app.UseForwardedHeaders(new ForwardedHeadersOptions {ForwardedHeaders = ForwardedHeaders.XForwardedProto});

            loggerFactory.AddNLog();

            app.UseLogApiRequest();

            // Shows UseCors with CorsPolicyBuilder.
            app.UseCors(builder =>
                builder.WithOrigins("https://stg-status.cb.com", "https://status.cb.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c => { c.RouteTemplate = "{documentName}/swagger.json"; });
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/docs/swagger.json", "Template Api"); });

            app.UseMvc();
        }

        public virtual void BindIdentityServerAuthenticationOptions(IdentityServerAuthenticationOptions options)
        {
            Configuration.GetSection("IdentityProvider").Bind(options);
        }
    }
}