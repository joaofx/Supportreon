using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Miru;
using Miru.Databases.EntityFramework;
using Miru.Foundation.Hosting;
using Miru.Foundation.Logging;
using Miru.Mailing;
using Miru.Mvc;
using Miru.Pipeline;
using Miru.Queuing;
using Miru.Turbolinks;
using Miru.Userfy;
using Serilog.Events;
using Supportreon.Config;
using Supportreon.Database;
using Supportreon.Domain;

namespace Supportreon
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMiru<Startup>()

                // log
                .AddSerilogConfig(config =>
                {
                    // config.AspNet(LogEventLevel.Debug);
                    // config.EfCoreSql(LogEventLevel.Information);
                })

                // pipeline
                .AddDefaultPipeline<Startup>()

                // database
                .AddEfCoreSqlite<SupportreonDbContext>()

                // user security
                .AddUserfy<User>(options =>
                {
                    options.LoginPath = "/Accounts/Login";
                })
                .AddAuthorization<AuthorizationConfig>()

                // mailing
                .AddMailing(_ =>
                {
                    _.EmailDefaults(email => email.From("noreply@Supportreon.com", "Supportreon"));
                })
                .AddSenderSmtp()
                // .AddSenderStorage()

                // queuing
                .AddQueuing(_ =>
                {
                    _.UseLiteDb();
                })
                .AddHangfireServer()

                .AddTurbolinks();

                // your app services
                // services.AddSingleton<IService, Service>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // The Middlewares here are configured in order of executation
            // Here, they are organized for Miru defaults. Changing the order might break some functionality 

            if (env.IsDevelopmentOrTest())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            
            app.UseStaticFiles();
            
            app.UseRequestLogging();
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseExceptionLogging();

            app.UseRouting();
            app.UseAuthentication();
            app.UseHangfireDashboard();
            
            app.UseEndpoints(e =>
            {
                e.MapDefaultControllerRoute();
            });
        }
    }
}
