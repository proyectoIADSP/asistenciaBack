using asistenciaBack.Identity.Application.Dtos;
using asistenciaBack.Identity.Application.Interfaces;
using asistenciaBack.Identity.Domain.Entities;
using asistenciaBack.Identity.Domain.Enums;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Identity.Application.Commands;

public class RegisterCommand
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommand(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> ExecuteAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure<AuthResponse>("Nombre, email y contraseña son obligatorios.");
        }

        if (request.Password.Length < 6)
        {
            return Result.Failure<AuthResponse>("La contraseña debe tener al menos 6 caracteres.");
        }

        if (!Enum.IsDefined(request.Role) ||
            (request.Role != UserRole.Deacon && request.Role != UserRole.Administrator))
        {
            return Result.Failure<AuthResponse>("Rol inválido. Use Deacon o Administrator.");
        }

        if (await _users.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return Result.Failure<AuthResponse>("Ya existe un usuario con ese email.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.FullName, request.Email, passwordHash, request.Role);

        await _users.AddAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.CreateToken(user);

        return Result.Success(new AuthResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            token));
    }
}
