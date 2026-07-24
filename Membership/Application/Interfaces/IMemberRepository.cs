using asistenciaBack.Membership.Domain.Entities;

namespace asistenciaBack.Membership.Application.Interfaces;

public interface IMemberRepository
{
    Task<IReadOnlyList<Member>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Member member, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
