using asistenciaBack.Attendance.Application.Interfaces;
using asistenciaBack.Attendance.Domain;
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

    public async Task<IReadOnlyList<int>> GetExistingMemberIdsForDateAsync(
        DateOnly date,
        IReadOnlyCollection<int> memberIds,
        CancellationToken cancellationToken = default)
    {
        if (memberIds.Count == 0)
        {
            return Array.Empty<int>();
        }

        return await _db.AttendanceRecords
            .AsNoTracking()
            .Where(r => r.Date == date && memberIds.Contains(r.MemberId))
            .Select(r => r.MemberId)
            .ToListAsync(cancellationToken);
    }

    public async Task InsertBulkAsync(
        IReadOnlyList<AttendanceRecord> records,
        CancellationToken cancellationToken = default)
    {
        if (records.Count == 0)
        {
            return;
        }

        await _db.AttendanceRecords.AddRangeAsync(records, cancellationToken);
    }

    public async Task<IReadOnlyList<(int MemberId, int Present, int Late, int Absent)>> GetMonthlyStatsAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var saturdays = SaturdayCalendar.GetSaturdaysInMonth(year, month);
        if (saturdays.Count == 0)
        {
            return Array.Empty<(int, int, int, int)>();
        }

        var rows = await _db.AttendanceRecords
            .AsNoTracking()
            .Where(r => saturdays.Contains(r.Date))
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
