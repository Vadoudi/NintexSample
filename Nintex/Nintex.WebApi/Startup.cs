using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nintex.Business.Service;
using Nintex.Service.Caching;
using Nintex.Service.Filing;
using Nintex.WebApi.Models;

namespace Nintex.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt => opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());

            services.Configure<WebsiteSettings>(Configuration.GetSection("WebsiteSettings"));

            using (var serviceScope = services.BuildServiceProvider().CreateScope())
            {
                var websiteSettings = serviceScope.ServiceProvider.GetService<IOptions<WebsiteSettings>>();

                switch (websiteSettings.Value.UrlShorteningServiceType)
                {
                    case Business.UrlShorteningServiceType.Filing:
                        services.AddScoped(typeof(IUrlShorteningFileService), typeof(FilingUrlShorteningFileService));
                        break;

                    case Business.UrlShorteningServiceType.Caching:
                    default:
                        services.AddScoped(typeof(IUrlShorteningFileService), typeof(CachingUrlShorteningFileService));
                        break;
                }
            }

            services.AddScoped(typeof(IUrlShorteningService), typeof(FilingUrlShorteningService));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            //var builder = new ConfigurationBuilder()
            //.SetBasePath(env.ContentRootPath)
            // .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //Configuration = builder.Build();
        }
    }
}
