namespace asistenciaBack.Attendance.Domain;

public static class SaturdayCalendar
{
    public static bool IsSaturday(DateOnly date) =>
        date.DayOfWeek == DayOfWeek.Saturday;

    public static IReadOnlyList<DateOnly> GetSaturdaysInMonth(int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var saturdays = new List<DateOnly>();

        for (var day = 1; day <= daysInMonth; day++)
        {
            var date = new DateOnly(year, month, day);
            if (IsSaturday(date))
            {
                saturdays.Add(date);
            }
        }

        return saturdays;
    }
}
