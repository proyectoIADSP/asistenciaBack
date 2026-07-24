using asistenciaBack.Membership.Application;
using asistenciaBack.Membership.Application.Dtos;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Membership.Application.Commands;

public class DeactivateMemberCommand
{
    private readonly IMemberRepository _members;

    public DeactivateMemberCommand(IMemberRepository members)
    {
        _members = members;
    }

    public async Task<Result<MemberDto>> ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        var member = await _members.GetByIdAsync(id, cancellationToken);
        if (member is null)
        {
            return Result.Failure<MemberDto>("Miembro no encontrado.");
        }

        if (!member.IsActive)
        {
            return Result.Failure<MemberDto>("El miembro ya está inactivo.");
        }

        member.Deactivate();
        await _members.SaveChangesAsync(cancellationToken);

        return Result.Success(member.ToDto());
    }
}
