using asistenciaBack.Attendance.Application.Dtos;
using asistenciaBack.Attendance.Application.Interfaces;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Attendance.Application.Commands;

public class GetMonthlyAttendanceStatsCommand
{
    private readonly IAttendanceRepository _attendance;

    public GetMonthlyAttendanceStatsCommand(IAttendanceRepository attendance)
    {
        _attendance = attendance;
    }

    public async Task<Result<IReadOnlyList<MonthlyStatDto>>> ExecuteAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (year < 2000 || year > 2100)
        {
            return Result.Failure<IReadOnlyList<MonthlyStatDto>>("El año no es válido.");
        }

        if (month is < 1 or > 12)
        {
            return Result.Failure<IReadOnlyList<MonthlyStatDto>>("El mes debe estar entre 1 y 12.");
        }

        var stats = await _attendance.GetMonthlyStatsAsync(year, month, cancellationToken);
        var dtos = stats
            .Select(s => new MonthlyStatDto(s.MemberId, s.Present, s.Late, s.Absent))
            .ToList();

        return Result.Success<IReadOnlyList<MonthlyStatDto>>(dtos);
    }
}
