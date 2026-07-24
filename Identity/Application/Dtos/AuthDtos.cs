using asistenciaBack.Identity.Domain.Enums;

namespace asistenciaBack.Identity.Application.Dtos;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string FullName, string Email, string Password, UserRole Role);

public record AuthResponse(
    int UserId,
    string FullName,
    string Email,
    string Role,
    string Token);
