using asistenciaBack.Attendance.Domain.Enums;

namespace asistenciaBack.Attendance.Domain.Entities;

public class AttendanceRecord
{
    public int Id { get; private set; }
    public int MemberId { get; private set; }
    public DateOnly Date { get; private set; }
    public AttendanceStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public int RegisteredByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; private set; }

    private AttendanceRecord()
    {
    }

    public static AttendanceRecord Create(
        int memberId,
        DateOnly date,
        AttendanceStatus status,
        int registeredByUserId,
        string? notes = null)
    {
        return new AttendanceRecord
        {
            MemberId = memberId,
            Date = date,
            Status = status,
            RegisteredByUserId = registeredByUserId,
            Notes = NormalizeNotes(notes),
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(AttendanceStatus status, int registeredByUserId, string? notes)
    {
        Status = status;
        RegisteredByUserId = registeredByUserId;
        Notes = NormalizeNotes(notes);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? NormalizeNotes(string? notes) =>
        string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
}
