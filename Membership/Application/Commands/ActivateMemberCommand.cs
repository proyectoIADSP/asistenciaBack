using asistenciaBack.Membership.Application;
using asistenciaBack.Membership.Application.Dtos;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Membership.Application.Commands;

public class ActivateMemberCommand
{
    private readonly IMemberRepository _members;

    public ActivateMemberCommand(IMemberRepository members)
    {
        _members = members;
    }

    public async Task<Result<MemberDto>> ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        var member = await _members.GetByIdAsync(id, cancellationToken);
        if (member is null)
        {
            return Result.Failure<MemberDto>("Member not found.");
        }

        if (member.IsActive)
        {
            return Result.Failure<MemberDto>("Member is already active.");
        }

        member.Activate();
        await _members.SaveChangesAsync(cancellationToken);

        return Result.Success(member.ToDto());
    }
}
