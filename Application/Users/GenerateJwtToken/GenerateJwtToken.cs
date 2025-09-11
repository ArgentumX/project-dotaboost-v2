using Application.Common.Interfaces;
using MediatR;

namespace Application.Users.GenerateJwtToken;

public class GenerateJwtTokenCommand(UserDto user) : IRequest<String>
{
    public UserDto User { get; set; } = user;

    public class GenerateJwtTokenHandler : IRequestHandler<GenerateJwtTokenCommand, String>
    {
        private readonly ITokenService _tokenService;

        public GenerateJwtTokenHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<String> Handle(GenerateJwtTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _tokenService.GenerateJwtToken(request.User);
            return result;
        }
    }
}