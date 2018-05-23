using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.DockerSecrets;
using Microsoft.Extensions.DependencyInjection;

namespace TodoApi
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; }

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;

            Configuration = new ConfigurationBuilder()                
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddDockerSecrets(source => {
                    source.Optional = true;
                    source.IgnoreCondition = name => name.Equals("rhel7.repo");
                })
                .Build();
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