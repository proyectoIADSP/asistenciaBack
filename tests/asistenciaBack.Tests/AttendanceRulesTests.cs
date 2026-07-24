using asistenciaBack.Attendance.Application.Commands;
using asistenciaBack.Attendance.Application.Dtos;
using asistenciaBack.Attendance.Application.Interfaces;
using asistenciaBack.Attendance.Domain;
using asistenciaBack.Attendance.Domain.Entities;
using asistenciaBack.Attendance.Domain.Enums;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Membership.Domain.Entities;

namespace asistenciaBack.Tests;

public class SaturdayCalendarTests
{
    [Fact]
    public void July_2026_has_expected_saturdays()
    {
        var saturdays = SaturdayCalendar.GetSaturdaysInMonth(2026, 7);

        Assert.Equal(
            new[]
            {
                new DateOnly(2026, 7, 4),
                new DateOnly(2026, 7, 11),
                new DateOnly(2026, 7, 18),
                new DateOnly(2026, 7, 25)
            },
            saturdays);
    }

    [Theory]
    [InlineData(2026, 7, 4, true)]
    [InlineData(2026, 7, 24, false)]
    [InlineData(2026, 7, 25, true)]
    public void IsSaturday_works(int y, int m, int d, bool expected)
    {
        Assert.Equal(expected, SaturdayCalendar.IsSaturday(new DateOnly(y, m, d)));
    }
}

public class SaveBulkAttendanceCommandTests
{
    [Fact]
    public async Task Rejects_non_saturday()
    {
        var attendance = new FakeAttendanceRepository();
        var members = new FakeMemberRepository();
        members.AddActive(1);
        var sut = new SaveBulkAttendanceCommand(attendance, members);
        var friday = new DateOnly(2026, 7, 24);

        var result = await sut.ExecuteAsync(1, new BulkAttendanceRequest(friday, [
            new BulkAttendanceItemDto(1, AttendanceStatus.Present, null)
        ]));

        Assert.False(result.IsSuccess);
        Assert.Equal(SaveBulkAttendanceCommand.OnlySaturdaysError, result.Error);
    }

    [Fact]
    public async Task Rejects_duplicate_member_date_without_updating()
    {
        var attendance = new FakeAttendanceRepository();
        var members = new FakeMemberRepository();
        members.AddActive(1);
        var sut = new SaveBulkAttendanceCommand(attendance, members);
        var saturday = new DateOnly(2026, 7, 11);
        attendance.SeedExisting(AttendanceRecord.Create(1, saturday, AttendanceStatus.Present, 9));

        var result = await sut.ExecuteAsync(1, new BulkAttendanceRequest(saturday, [
            new BulkAttendanceItemDto(1, AttendanceStatus.Late, "intento cambiar")
        ]));

        Assert.False(result.IsSuccess);
        Assert.Equal(SaveBulkAttendanceCommand.AlreadyRegisteredError, result.Error);
        Assert.Single(attendance.Store);
        Assert.Equal(AttendanceStatus.Present, attendance.Store[0].Status);
    }

    [Fact]
    public async Task Inserts_when_not_exists()
    {
        var attendance = new FakeAttendanceRepository();
        var members = new FakeMemberRepository();
        members.AddActive(1);
        var sut = new SaveBulkAttendanceCommand(attendance, members);
        var saturday = new DateOnly(2026, 7, 18);

        var result = await sut.ExecuteAsync(1, new BulkAttendanceRequest(saturday, [
            new BulkAttendanceItemDto(1, AttendanceStatus.Present, null)
        ]));

        Assert.True(result.IsSuccess);
        Assert.Single(attendance.Store);
        Assert.Equal(AttendanceStatus.Present, attendance.Store[0].Status);
    }

    [Fact]
    public async Task Monthly_stats_ignore_non_saturday_rows()
    {
        var attendance = new FakeAttendanceRepository();
        attendance.SeedExisting(AttendanceRecord.Create(1, new DateOnly(2026, 7, 24), AttendanceStatus.Present, 1));
        attendance.SeedExisting(AttendanceRecord.Create(1, new DateOnly(2026, 7, 25), AttendanceStatus.Late, 1));

        var stats = await attendance.GetMonthlyStatsAsync(2026, 7);

        Assert.Single(stats);
        Assert.Equal(0, stats[0].Present);
        Assert.Equal(1, stats[0].Late);
        Assert.Equal(0, stats[0].Absent);
    }
}

internal sealed class FakeMemberRepository : IMemberRepository
{
    private readonly HashSet<int> _active = [];

    public void AddActive(int id) => _active.Add(id);

    public Task AddAsync(Member member, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<bool> ExistsByFullNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public Task<bool> ExistsByFullNameAsync(string firstName, string lastName, int excludeMemberId, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, int excludeMemberId, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public Task<IReadOnlyList<Member>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Member>>([]);

    public Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!_active.Contains(id))
        {
            return Task.FromResult<Member?>(null);
        }

        return Task.FromResult<Member?>(Member.Create("Test", "User", "300123456", null));
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal sealed class FakeAttendanceRepository : IAttendanceRepository
{
    public List<AttendanceRecord> Store { get; } = [];

    public void SeedExisting(AttendanceRecord record) => Store.Add(record);

    public Task<IReadOnlyList<AttendanceRecord>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<AttendanceRecord>>(Store.Where(r => r.Date == date).ToList());

    public Task<IReadOnlyList<int>> GetExistingMemberIdsForDateAsync(
        DateOnly date,
        IReadOnlyCollection<int> memberIds,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<int>>(
            Store.Where(r => r.Date == date && memberIds.Contains(r.MemberId)).Select(r => r.MemberId).ToList());

    public Task InsertBulkAsync(IReadOnlyList<AttendanceRecord> records, CancellationToken cancellationToken = default)
    {
        Store.AddRange(records);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<(int MemberId, int Present, int Late, int Absent)>> GetMonthlyStatsAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var saturdays = SaturdayCalendar.GetSaturdaysInMonth(year, month);
        var rows = Store
            .Where(r => saturdays.Contains(r.Date))
            .GroupBy(r => r.MemberId)
            .Select(g => (
                g.Key,
                g.Count(x => x.Status == AttendanceStatus.Present),
                g.Count(x => x.Status == AttendanceStatus.Late),
                g.Count(x => x.Status == AttendanceStatus.Absent)))
            .ToList();

        return Task.FromResult<IReadOnlyList<(int, int, int, int)>>(rows);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
