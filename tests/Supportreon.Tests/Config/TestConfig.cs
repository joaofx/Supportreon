using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miru.Fabrication;
using Miru.Foundation.Hosting;
using Miru.Testing;
using Miru.Testing.Userfy;
using Supportreon.Domain;
using Supportreon.Tests.Features.Donations;

namespace Supportreon.Tests.Config
{
    public class TestsConfig : ITestConfig
    {
        public virtual void ConfigureTestServices(IServiceCollection services)
        {
            services
                .AddFeatureTesting()
                .AddSqliteDatabaseCleaner()
                .AddFabrication<SupportreonFabricator>();

            // Mock your services that talk with external apps
            // services.Mock<IService>();
        }
    
        public void ConfigureRun(TestRunConfig run)
        {
            // This run configurations only works for tests that inherits MiruTest
            // It includes FeatureTest, PageTest, and all other Miru's types of tests
            
            run.BeforeSuite(_ =>
            {
                _.MigrateDatabase();
            });
    
            run.BeforeCase(_ =>
            {
                _.Logout();
                
                _.ClearFabricator();
                _.ClearDatabase();
                _.ClearQueue();
            });

            run.BeforeCase<AuthorizationTest>(_ =>
            {
                _.Logout();
            });
            
            run.BeforeCase<IRequiresAuthenticatedUser>(_ =>
            {
                _.MakeSavingLogin(_.Fab().Make<User>());
            });
        }
        
        public virtual IHostBuilder GetHostBuilder()
        {
            return MiruHost.CreateMiruHost<Startup>();
        }
    }
}
