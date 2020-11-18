using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miru.Behaviors;
using Miru.Mvc;
using Miru.Userfy;

namespace Supportreon.Features.Accounts
{
    public class AccountLogout 
    {
        public class Command : IRequest<Result>
        {
        }
        
        public class Result : IRedirectResult
        {
            public string RedirectTo { get; set; }
        }

        public class Handler : RequestHandler<Command, Result>
        {
            private readonly IUserSession _userSession;

            public Handler(IUserSession userSession) => _userSession = userSession;

            protected override Result Handle(Command request)
            {
                _userSession.Logout();
                
                return new Result() { RedirectTo = "/" };
            }
        }

        public class AccountsController : MiruController
        {
            [HttpPost]
            public async Task<Result> Logout(Command command) => await Send(command);
        }
    }
}
