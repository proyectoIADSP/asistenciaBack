using asistenciaBack.Membership.Domain.Entities;

namespace asistenciaBack.Membership.Application.Interfaces;

public interface IMemberRepository
{
    Task<IReadOnlyList<Member>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Member>> GetAllInactiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Member>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByFullNameAsync(
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByFullNameAsync(
        string firstName,
        string lastName,
        int excludeMemberId,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneNumberAsync(
        string phoneNumber,
        int excludeMemberId,
        CancellationToken cancellationToken = default);
    Task AddAsync(Member member, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
