﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudioServices.Controllers.Accounting;
using StudioServices.Controllers.Items;
using StudioServices.Controllers.Newsboard;
using StudioServices.Controllers.Persons;
using StudioServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioServicesWeb.DataControllers;

namespace StudioServicesWeb
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
            StudioServicesConfig.Init();
            services.AddMvc().
                AddJsonOptions(options =>
                { 
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var sqlserver_connection = Configuration.GetConnectionString("SQLServerConnection");
            
            var dbContextOptions = (new DbContextOptionsBuilder().UseSqlServer(sqlserver_connection)).Options;
            StudioServicesDBContext.CtxOptions = dbContextOptions;
            services.AddDbContext<StudioServicesDBContext>(/*options => options.UseSqlServer(sqlserver_connection)*/);

            services.AddSession(o =>
            {
                o.Cookie.Name = "SSCookie";
                o.Cookie.HttpOnly = true;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            //services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            
            services.AddDistributedRedisCache(o =>
            {
                o.Configuration = Configuration.GetConnectionString("Redis");
            });
            
            services.AddResponseCompression();

            services.AddTransient<DatabaseEF>();
            services.AddTransient<AuthenticationManager>();
            services.AddTransient<PersonsManager>();
            services.AddTransient<ItemsManager>();
            services.AddTransient<NewsboardManager>();
            services.AddTransient<WarehouseManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}
            SessionOptions sessionOptions = new SessionOptions()
            {
                IdleTimeout = TimeSpan.FromDays(7),
                Cookie = new CookieBuilder()
                {
                    Name = "SSWSESSID",
                    Expiration = TimeSpan.FromDays(90)
                }
            };
			// app.UseHttpsRedirection();
			
            app.UseSession(sessionOptions);

            app.UseMvc();
        }
    }
}
