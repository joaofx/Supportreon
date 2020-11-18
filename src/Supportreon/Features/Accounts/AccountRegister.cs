using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Miru;
using Miru.Behaviors;
using Miru.Mailing;
using Miru.Mvc;
using Miru.Validation;
using Supportreon.Database;
using Supportreon.Domain;

namespace Supportreon.Features.Accounts
{
    public class AccountRegister
    {
        public class Query : IRequest<Command>
        {
            public string ReturnUrl { get; set; }
        }

        public class Command : IRequest<Result>
        {
            public string Name { get; set; }
            public string Email { get; set; }  
            public string Password { get; set; }
            public string PasswordConfirmation { get; set; }
            public string ReturnUrl { get; set; }
        }

        public class Result
        {
            public User User { get; set; }
            public string ReturnUrl { get; set; }
        }

        public class Handler : 
            RequestHandler<Query, Command>,
            IRequestHandler<Command, Result>
        {
            private readonly SupportreonDbContext _db;
            private readonly IMailer _mailer;

            public Handler(SupportreonDbContext db, IMailer mailer)
            {
                _db = db;
                _mailer = mailer;
            }

            protected override Command Handle(Query request)
            {
                return new Command
                {
                    ReturnUrl = request.ReturnUrl
                };
            }

            public async Task<Result> Handle(Command command, CancellationToken ct)
            {
                var user = new User
                {
                    Name = command.Name,
                    Email = command.Email,
                    HashedPassword = Hash.Create(command.Password)
                };

                await _db.Users.AddAsync(user, ct);

                await _mailer.SendLater(new AccountRegisteredMail(user));
                
                return new Result
                {
                    ReturnUrl = command.ReturnUrl,
                    User = user
                };
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(SupportreonDbContext db)
            {
                RuleFor(x => x.Name).NotEmpty();
                
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .UniqueAsync(async (cmd, ct) => 
                        await db.Users.UniqueAsync(0, m => m.Email == cmd.Email, ct));

                RuleFor(x => x.Password).NotEmpty().Equal(x => x.PasswordConfirmation).WithMessage("The password and confirmation password do not match");
                
                RuleFor(x => x.PasswordConfirmation).NotEmpty();
            }
        }

        public class AccountsController : MiruController
        {
            public async Task<Command> Register(Query query) => await Send(query);

            [HttpPost]
            public async Task<Result> Register(Command command) => await Send(command);
        }
    }
}
