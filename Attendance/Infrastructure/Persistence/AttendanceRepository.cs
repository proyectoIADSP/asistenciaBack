using asistenciaBack.Attendance.Application.Interfaces;
using asistenciaBack.Attendance.Domain.Entities;
using asistenciaBack.Attendance.Domain.Enums;
using asistenciaBack.Database;
using Microsoft.EntityFrameworkCore;

namespace asistenciaBack.Attendance.Infrastructure.Persistence;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _db;

    public AttendanceRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<AttendanceRecord>> GetByDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _db.AttendanceRecords
            .AsNoTracking()
            .Where(r => r.Date == date)
            .OrderBy(r => r.MemberId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertBulkAsync(
        IReadOnlyList<AttendanceRecord> records,
        CancellationToken cancellationToken = default)
    {
        if (records.Count == 0)
        {
            return;
        }

        var date = records[0].Date;
        var memberIds = records.Select(r => r.MemberId).ToList();

        var existing = await _db.AttendanceRecords
            .Where(r => r.Date == date && memberIds.Contains(r.MemberId))
            .ToListAsync(cancellationToken);

        var existingByMemberId = existing.ToDictionary(r => r.MemberId);

        foreach (var incoming in records)
        {
            if (existingByMemberId.TryGetValue(incoming.MemberId, out var current))
            {
                current.Update(incoming.Status, incoming.RegisteredByUserId, incoming.Notes);
            }
            else
            {
                await _db.AttendanceRecords.AddAsync(incoming, cancellationToken);
            }
        }
    }

    public async Task<IReadOnlyList<(int MemberId, int Present, int Late, int Absent)>> GetMonthlyStatsAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1);

        var rows = await _db.AttendanceRecords
            .AsNoTracking()
            .Where(r => r.Date >= start && r.Date < end)
            .GroupBy(r => r.MemberId)
            .Select(g => new
            {
                MemberId = g.Key,
                Present = g.Count(x => x.Status == AttendanceStatus.Present),
                Late = g.Count(x => x.Status == AttendanceStatus.Late),
                Absent = g.Count(x => x.Status == AttendanceStatus.Absent)
            })
            .OrderBy(x => x.MemberId)
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => (x.MemberId, x.Present, x.Late, x.Absent))
            .ToList();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
