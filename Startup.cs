using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.DockerSecrets;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TodoApi
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; }

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
                
            if (env.IsProduction())
            {
                var sf = Environment.GetEnvironmentVariable("SECRET_CONFIG_FILE");
                if (!string.IsNullOrEmpty(sf))
                {
                    builder.AddJsonFile(sf, optional: true);
                }
            }

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<StatusSettings>(Configuration.GetSection("StatusSettings"));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}