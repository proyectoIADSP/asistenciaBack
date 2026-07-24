using asistenciaBack.Membership.Application;
using asistenciaBack.Membership.Application.Dtos;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Membership.Application.Commands;

public class GetMembersCommand
{
    private readonly IMemberRepository _members;

    public GetMembersCommand(IMemberRepository members)
    {
        _members = members;
    }

    public async Task<Result<IReadOnlyList<MemberDto>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var members = await _members.GetAllActiveAsync(cancellationToken);
        var dtos = members.Select(m => m.ToDto()).ToList();
        return Result.Success<IReadOnlyList<MemberDto>>(dtos);
    }
}
