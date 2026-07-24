using asistenciaBack.Attendance.Domain;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Attendance.Application.Commands;

public class GetSaturdaysOfMonthCommand
{
    public Result<IReadOnlyList<DateOnly>> Execute(int year, int month)
    {
        if (year < 2000 || year > 2100)
        {
            return Result.Failure<IReadOnlyList<DateOnly>>("El año no es válido.");
        }

        if (month is < 1 or > 12)
        {
            return Result.Failure<IReadOnlyList<DateOnly>>("El mes debe estar entre 1 y 12.");
        }

        return Result.Success(SaturdayCalendar.GetSaturdaysInMonth(year, month));
    }
}
