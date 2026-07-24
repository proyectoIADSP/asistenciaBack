using asistenciaBack.Identity.Application.Dtos;
using asistenciaBack.Identity.Application.Interfaces;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Identity.Application.Commands;

public class LoginCommand
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommand(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> ExecuteAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure<AuthResponse>("Email y contraseña son obligatorios.");
        }

        var user = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result.Failure<AuthResponse>("Credenciales inválidas.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResponse>("Credenciales inválidas.");
        }

        var token = _jwtTokenService.CreateToken(user);

        return Result.Success(new AuthResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            token));
    }
}
