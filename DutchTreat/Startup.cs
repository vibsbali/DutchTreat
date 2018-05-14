using System.Text;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DutchTreat
{
   public class Startup
   {
      private readonly IConfiguration _config;
      private readonly IHostingEnvironment _hostingEnvironment;

      public Startup(IConfiguration config, IHostingEnvironment hostingEnvironment)
      {
         this._config = config;
         _hostingEnvironment = hostingEnvironment;
      }
      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices(IServiceCollection services)
      {

         services.AddIdentity<StoreUser, IdentityRole>(cfg => { cfg.User.RequireUniqueEmail = true; })
            .AddEntityFrameworkStores<DutchContext>();

         //we are supporting both cookie and jwt 
         services.AddAuthentication()
            .AddCookie()
            .AddJwtBearer(cfg =>
            {
               cfg.TokenValidationParameters = new TokenValidationParameters()
               {
                  ValidIssuer = _config["Tokens:Issuer"],
                  ValidAudience = _config["Tokens:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
               };

            });

         services.AddDbContext<DutchContext>(cfg =>
         {
            cfg.UseSqlServer(_config.GetConnectionString("DutchConnectionString"));
         });

         services.AddAutoMapper();

         services.AddTransient<IMailService, NullMailService>();
         services.AddTransient<DutchSeeder>();
         services.AddScoped<IDutchRepository, DutchRepository>();

         var mvcBuilder = services.AddMvc(opt =>
         {
            if (_hostingEnvironment.IsProduction())
            {
               opt.Filters.Add(new RequireHttpsAttribute());
            }
         });
         //ignore the self reference loop
         mvcBuilder.AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);


         services.AddLogging();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         //app.UseDefaultFiles();
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }
         else
         {
            app.UseExceptionHandler("/error");
         }


         app.UseStaticFiles();

         //Before MVC important!
         app.UseAuthentication();

         app.UseMvc(cfg =>
         {
            cfg.MapRoute("Default",
                   "{controller}/{action}/{id?}",
                   new { Controller = "App", Action = "Index" });
         });
         //app.Run(async (context) =>
         //{
         //    await context.Response.WriteAsync("Hello World!");
         //});

         if (env.IsDevelopment())
         {
            //See the database
            using (var scope = app.ApplicationServices.CreateScope())
            {
               var seeder = scope.ServiceProvider.GetService<DutchSeeder>();
               seeder.Seed().Wait();
            }
         }
      }
   }
}
