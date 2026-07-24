using asistenciaBack.Membership.Application.Dtos;
using asistenciaBack.Membership.Domain.Entities;

namespace asistenciaBack.Membership.Application;

internal static class MemberMappings
{
    public static MemberDto ToDto(this Member member) =>
        new(
            member.Id,
            member.FirstName,
            member.LastName,
            member.PhoneNumber,
            member.Address,
            member.IsActive);
}
