using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miru.Core;
using Miru.Foundation.Logging;
using Miru.PageTesting;
using Miru.PageTesting.Chrome;
using Miru.Storages;
using Miru.Testing;
using Miru.Testing.Userfy;
using OpenQA.Selenium.Chrome;
using Serilog.Events;
using Supportreon.Domain;
using Supportreon.Tests.Config;

namespace Supportreon.PageTests.Config
{
    public class PageTestsConfig : ITestConfig
    {
        public void ConfigureTestServices(IServiceCollection services)
        {
            // import services from Supportreon.Tests
            services.AddFrom<TestsConfig>();
            
            services.AddPageTesting(options =>
            {
                if (OS.IsWindows)
                    options.UseChrome(new ChromeOptions().Incognito());
                else
                    options.UseChrome(new ChromeOptions().Incognito().Headless());
            });

            services.AddSingleton(sp => TestLoggerConfigurations.ForPageTest(sp.GetService<Storage>()));

            services.AddSerilogConfig(cfg =>
            {
                cfg.EfCoreSql(LogEventLevel.Debug);
            });
        }

        public void ConfigureRun(TestRunConfig run)
        {
            run.PageTestingDefault();
            
            run.BeforeAll<IRequiresAuthenticatedUser>(_ =>
            {
                _.MakeSavingLogin<User>();
            });
        }
        
        public IHostBuilder GetHostBuilder() => 
            TestMiruHost.CreateMiruHost<Startup>();
    }
}
