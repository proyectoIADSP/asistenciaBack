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

    public async Task AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        await _db.Members.AddAsync(member, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
