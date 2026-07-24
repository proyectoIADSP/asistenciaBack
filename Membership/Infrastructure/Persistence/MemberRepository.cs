using asistenciaBack.Database;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Membership.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace asistenciaBack.Membership.Infrastructure.Persistence;

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _db;

    public MemberRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Member>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Members
            .AsNoTracking()
            .Where(m => m.IsActive)
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToListAsync(cancellationToken);
    }

    public Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Members.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public Task<bool> ExistsByFullNameAsync(
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var normalizedFirst = firstName.Trim().ToLowerInvariant();
        var normalizedLast = lastName.Trim().ToLowerInvariant();

        return _db.Members.AnyAsync(
            m => m.FirstName.ToLower() == normalizedFirst && m.LastName.ToLower() == normalizedLast,
            cancellationToken);
    }

    public Task<bool> ExistsByFullNameAsync(
        string firstName,
        string lastName,
        int excludeMemberId,
        CancellationToken cancellationToken = default)
    {
        var normalizedFirst = firstName.Trim().ToLowerInvariant();
        var normalizedLast = lastName.Trim().ToLowerInvariant();

        return _db.Members.AnyAsync(
            m => m.Id != excludeMemberId
                 && m.FirstName.ToLower() == normalizedFirst
                 && m.LastName.ToLower() == normalizedLast,
            cancellationToken);
    }

    public Task<bool> ExistsByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var normalized = phoneNumber.Trim();
        return _db.Members.AnyAsync(m => m.PhoneNumber == normalized, cancellationToken);
    }

    public Task<bool> ExistsByPhoneNumberAsync(
        string phoneNumber,
        int excludeMemberId,
        CancellationToken cancellationToken = default)
    {
        var normalized = phoneNumber.Trim();
        return _db.Members.AnyAsync(
            m => m.Id != excludeMemberId && m.PhoneNumber == normalized,
            cancellationToken);
    }

    public async Task AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        await _db.Members.AddAsync(member, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
