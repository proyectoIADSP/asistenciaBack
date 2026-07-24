using asistenciaBack.Attendance.Application.Dtos;
using asistenciaBack.Attendance.Domain.Entities;

namespace asistenciaBack.Attendance.Application;

internal static class AttendanceMappings
{
    public static AttendanceRecordDto ToDto(this AttendanceRecord record) =>
        new(
            record.Id,
            record.MemberId,
            record.Date,
            record.Status,
            record.Notes,
            record.RegisteredByUserId);
}
