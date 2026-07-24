using asistenciaBack.Attendance.Application.Commands;
using asistenciaBack.Attendance.Application.Interfaces;
using asistenciaBack.Attendance.Infrastructure.Persistence;

namespace asistenciaBack.Attendance.Presentation;

public static class AttendanceDependencyInjection
{
    public static IServiceCollection AddAttendanceModule(this IServiceCollection services)
    {
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<GetAttendanceByDateCommand>();
        services.AddScoped<SaveBulkAttendanceCommand>();
        services.AddScoped<GetMonthlyAttendanceStatsCommand>();
        services.AddScoped<GetSaturdaysOfMonthCommand>();

        return services;
    }
}
