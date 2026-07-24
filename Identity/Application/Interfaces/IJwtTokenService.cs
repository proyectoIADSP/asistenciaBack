using asistenciaBack.Identity.Domain.Entities;

namespace asistenciaBack.Identity.Application.Interfaces;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
