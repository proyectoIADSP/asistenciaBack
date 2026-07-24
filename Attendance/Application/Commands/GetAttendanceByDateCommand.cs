using asistenciaBack.Attendance.Application;
using asistenciaBack.Attendance.Application.Dtos;
using asistenciaBack.Attendance.Application.Interfaces;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Attendance.Application.Commands;

public class GetAttendanceByDateCommand
{
    private readonly IAttendanceRepository _attendance;

    public GetAttendanceByDateCommand(IAttendanceRepository attendance)
    {
        _attendance = attendance;
    }

    public async Task<Result<IReadOnlyList<AttendanceRecordDto>>> ExecuteAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var records = await _attendance.GetByDateAsync(date, cancellationToken);
        var dtos = records.Select(r => r.ToDto()).ToList();
        return Result.Success<IReadOnlyList<AttendanceRecordDto>>(dtos);
    }
}
