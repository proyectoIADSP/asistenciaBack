using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using asistenciaBack.Attendance.Application.Commands;
using asistenciaBack.Attendance.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asistenciaBack.Attendance.Presentation.Controllers;

[ApiController]
[Route("api/attendance")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly GetAttendanceByDateCommand _getByDate;
    private readonly SaveBulkAttendanceCommand _saveBulk;
    private readonly GetMonthlyAttendanceStatsCommand _getStats;
    private readonly GetSaturdaysOfMonthCommand _getSaturdays;

    public AttendanceController(
        GetAttendanceByDateCommand getByDate,
        SaveBulkAttendanceCommand saveBulk,
        GetMonthlyAttendanceStatsCommand getStats,
        GetSaturdaysOfMonthCommand getSaturdays)
    {
        _getByDate = getByDate;
        _saveBulk = saveBulk;
        _getStats = getStats;
        _getSaturdays = getSaturdays;
    }

    /// <summary>Lista los sábados del mes (ej. julio 2026 → 4, 11, 18, 25).</summary>
    [HttpGet("saturdays/{year:int}/{month:int}")]
    public IActionResult GetSaturdays(int year, int month)
    {
        var result = _getSaturdays.Execute(year, month);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("by-date/{date}")]
    public async Task<IActionResult> GetByDate(DateOnly date, CancellationToken cancellationToken)
    {
        var result = await _getByDate.ExecuteAsync(date, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> SaveBulk(
        [FromBody] BulkAttendanceRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { error = "No se pudo obtener el id del usuario autenticado." });
        }

        var result = await _saveBulk.ExecuteAsync(userId, request, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("stats/{year:int}/{month:int}")]
    public async Task<IActionResult> GetMonthlyStats(
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var result = await _getStats.ExecuteAsync(year, month, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    private bool TryGetUserId(out int userId)
    {
        var raw =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return int.TryParse(raw, out userId);
    }
}
