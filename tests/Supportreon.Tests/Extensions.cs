using System;
using System.Threading.Tasks;
using MediatR;
using Miru;
using Miru.Security;
using Miru.Testing;
using Miru.Userfy;
using Supportreon.Database;
using Shouldly;

namespace Supportreon.Tests
{
    public static class Extensions
    {
        public static TReturn Db<TReturn>(this ITestFixture fixture, Func<SupportreonDbContext, TReturn> func)
        {
            return fixture.WithDb(func);
        }
        
        public static SupportreonFabricator Fab(this ITestFixture fixture)
        {
            return fixture.Get<SupportreonFabricator>();
        }
        
        public static async Task ShouldNotAuthorize<TResult, TUser>(
            this ITestFixture fixture, 
            TUser user,
            IRequest<TResult> message) where TUser : IUser
        {
            fixture.LoginAs(user);
            
            await Should.ThrowAsync<UnauthorizedException>(async () => await fixture.App.Send(message));
        }
        
        public static async Task ShouldNotAuthorizeAnonymous<TResult>(
            this ITestFixture fixture, 
            IRequest<TResult> message)
        {
            fixture.Logout();

            await Should.ThrowAsync<UnauthorizedException>(async () => await fixture.App.Send(message));
        }
        
        public static async Task<TResult> ShouldAuthorizeAnonymous<TResult>(
            this ITestFixture fixture, 
            IRequest<TResult> message)
        {
            fixture.Logout();
            
            return await fixture.App.Send(message);
        }
        
        public static async Task<TResult> ShouldAuthorize<TResult, TUser>(
            this ITestFixture fixture, 
            TUser user,
            IRequest<TResult> message) where TUser : IUser
        {
            fixture.LoginAs(user);
            
            return await fixture.App.Send(message);
        }
    }
}
