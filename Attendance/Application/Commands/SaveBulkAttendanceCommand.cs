using asistenciaBack.Attendance.Application;
using asistenciaBack.Attendance.Application.Dtos;
using asistenciaBack.Attendance.Application.Interfaces;
using asistenciaBack.Attendance.Domain;
using asistenciaBack.Attendance.Domain.Entities;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Attendance.Application.Commands;

public class SaveBulkAttendanceCommand
{
    public const string AlreadyRegisteredError =
        "La asistencia de este miembro para esa fecha ya fue registrada y no se puede modificar.";

    public const string OnlySaturdaysError =
        "La asistencia solo se registra los sábados.";

    private readonly IAttendanceRepository _attendance;
    private readonly IMemberRepository _members;

    public SaveBulkAttendanceCommand(
        IAttendanceRepository attendance,
        IMemberRepository members)
    {
        _attendance = attendance;
        _members = members;
    }

    public async Task<Result<IReadOnlyList<AttendanceRecordDto>>> ExecuteAsync(
        int registeredByUserId,
        BulkAttendanceRequest request,
        CancellationToken cancellationToken = default)
    {
        if (registeredByUserId <= 0)
        {
            return Result.Failure<IReadOnlyList<AttendanceRecordDto>>("Usuario autenticado inválido.");
        }

        if (!SaturdayCalendar.IsSaturday(request.Date))
        {
            return Result.Failure<IReadOnlyList<AttendanceRecordDto>>(OnlySaturdaysError);
        }

        if (request.Records is null || request.Records.Count == 0)
        {
            return Result.Failure<IReadOnlyList<AttendanceRecordDto>>("Debe enviar al menos un registro de asistencia.");
        }

        var duplicateMemberIds = request.Records
            .GroupBy(r => r.MemberId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateMemberIds.Count > 0)
        {
            return Result.Failure<IReadOnlyList<AttendanceRecordDto>>(
                $"MemberId duplicado en la solicitud: {string.Join(", ", duplicateMemberIds)}.");
        }

        foreach (var item in request.Records)
        {
            if (item.MemberId <= 0)
            {
                return Result.Failure<IReadOnlyList<AttendanceRecordDto>>("El MemberId debe ser mayor que cero.");
            }

            if (!Enum.IsDefined(item.Status))
            {
                return Result.Failure<IReadOnlyList<AttendanceRecordDto>>($"Estado inválido para el miembro {item.MemberId}.");
            }

            if (item.Notes is not null && item.Notes.Trim().Length > 100)
            {
                return Result.Failure<IReadOnlyList<AttendanceRecordDto>>(
                    $"Las notas del miembro {item.MemberId} deben tener máximo 100 caracteres.");
            }

            var member = await _members.GetByIdAsync(item.MemberId, cancellationToken);
            if (member is null || !member.IsActive)
            {
                return Result.Failure<IReadOnlyList<AttendanceRecordDto>>(
                    $"No se encontró un miembro activo con Id {item.MemberId}.");
            }
        }

        var memberIds = request.Records.Select(r => r.MemberId).ToList();
        var alreadyRegistered = await _attendance.GetExistingMemberIdsForDateAsync(
            request.Date,
            memberIds,
            cancellationToken);

        if (alreadyRegistered.Count > 0)
        {
            return Result.Failure<IReadOnlyList<AttendanceRecordDto>>(AlreadyRegisteredError);
        }

        var records = request.Records
            .Select(item => AttendanceRecord.Create(
                item.MemberId,
                request.Date,
                item.Status,
                registeredByUserId,
                item.Notes))
            .ToList();

        await _attendance.InsertBulkAsync(records, cancellationToken);
        await _attendance.SaveChangesAsync(cancellationToken);

        var saved = await _attendance.GetByDateAsync(request.Date, cancellationToken);
        var dtos = saved.Select(r => r.ToDto()).ToList();
        return Result.Success<IReadOnlyList<AttendanceRecordDto>>(dtos);
    }
}
