using asistenciaBack.Attendance.Domain.Entities;

namespace asistenciaBack.Attendance.Application.Interfaces;

public interface IAttendanceRepository
{
    Task<IReadOnlyList<AttendanceRecord>> GetByDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetExistingMemberIdsForDateAsync(
        DateOnly date,
        IReadOnlyCollection<int> memberIds,
        CancellationToken cancellationToken = default);

    /// <summary>Inserta registros nuevos. No actualiza existentes.</summary>
    Task InsertBulkAsync(
        IReadOnlyList<AttendanceRecord> records,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(int MemberId, int Present, int Late, int Absent)>> GetMonthlyStatsAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
