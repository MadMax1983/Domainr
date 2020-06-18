using Domainr.Manager.Core.Data;
using Domainr.Manager.Grpc.Services;
using Domainr.Manager.WebApi.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Domainr.Manager
{
    internal sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddControllers();

            services.AddSingleton<IEndpointRepository, InMemoryEndpointRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(policy =>
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<EndpointsService>();

                endpoints.MapControllers();
            });
        }
    }
}