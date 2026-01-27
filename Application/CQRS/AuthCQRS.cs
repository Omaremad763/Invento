using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS.Auth_DTOS;

using FluentValidation;

using MediatR;

namespace Application.CQRS;

    //commands
    public record RegisterUserCommand(RegisterDto RegisterDto) : IRequest<RegisterResponse>;
    public record ConfirmEmailCommand(ConfirmEmailDTO ConfirmEmailDto) : IRequest<ConfirmResponse>;
    public record LoginCommand(LoginDto LoginDto) : IRequest<LoginResponse>;
    public record ExternalAuthCommand(string code) : IRequest<LoginResponse>;

//validators

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.RegisterDto.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.RegisterDto.UserName).NotEmpty().MinimumLength(3);
        RuleFor(x => x.RegisterDto.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Must contain uppercase")
            .Matches(@"[a-z]").WithMessage("Must contain lowercase")
            .Matches(@"[0-9]").WithMessage("Must contain number");
    }
}

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.ConfirmEmailDto.UserID).NotEqual(Guid.Empty);
        RuleFor(x => x.ConfirmEmailDto.Token).NotEmpty();
    }
}
public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.LoginDto.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.LoginDto.Password).NotEmpty();
    }
}

public class ExternalAuthValidator : AbstractValidator<ExternalAuthCommand>
{
    public ExternalAuthValidator()
    {
        RuleFor(x => x.code).NotNull().NotEmpty();
    }

    //handlers
    public class RegisterUserHandler :
        IRequestHandler<RegisterUserCommand, RegisterResponse>,
        IRequestHandler<ConfirmEmailCommand, ConfirmResponse>,
        IRequestHandler<LoginCommand, LoginResponse>,
        IRequestHandler<ExternalAuthCommand, LoginResponse>

    {
        private readonly IInventoServices _services;
        private readonly IExternalAuthService _externalAuthService;
        public RegisterUserHandler(IInventoServices services, IExternalAuthService externalAuthService)
        {
            _services = services;
            _externalAuthService = externalAuthService;

        }

        public async Task<RegisterResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            return await _services.AuthService.RegisterAsync(request.RegisterDto);
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _services.AuthService.LoginAsync(request: request.LoginDto);
        }

        public async Task<ConfirmResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            return await _services.AuthService.ConfirmEmailAsync(request.ConfirmEmailDto);
        }


        public async Task<LoginResponse> Handle(ExternalAuthCommand request, CancellationToken cancellationToken)
        {
            return await _externalAuthService.GitHubAuth(request.code);
        }
    }
}
