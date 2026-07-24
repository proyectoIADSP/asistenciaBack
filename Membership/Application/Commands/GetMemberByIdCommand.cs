using asistenciaBack.Membership.Application;
using asistenciaBack.Membership.Application.Dtos;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Membership.Application.Commands;

public class GetMemberByIdCommand
{
    private readonly IMemberRepository _members;

    public GetMemberByIdCommand(IMemberRepository members)
    {
        _members = members;
    }

    public async Task<Result<MemberDto>> ExecuteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var member = await _members.GetByIdAsync(id, cancellationToken);
        if (member is null || !member.IsActive)
        {
            return Result.Failure<MemberDto>("Miembro no encontrado.");
        }

        return Result.Success(member.ToDto());
    }
}
