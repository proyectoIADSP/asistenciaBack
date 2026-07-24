namespace asistenciaBack.Membership.Application.Dtos;

public record MemberDto(
    int Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Address,
    bool IsActive);

public record CreateMemberRequest(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Address);

public record UpdateMemberRequest(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Address);
