using asistenciaBack.Attendance.Domain.Enums;

namespace asistenciaBack.Attendance.Application.Dtos;

public record AttendanceRecordDto(
    int Id,
    int MemberId,
    DateOnly Date,
    AttendanceStatus Status,
    string? Notes,
    int RegisteredByUserId);

public record BulkAttendanceItemDto(
    int MemberId,
    AttendanceStatus Status,
    string? Notes);

public record BulkAttendanceRequest(
    DateOnly Date,
    List<BulkAttendanceItemDto> Records);

public record MonthlyStatDto(
    int MemberId,
    int TotalPresent,
    int TotalLate,
    int TotalAbsent);
