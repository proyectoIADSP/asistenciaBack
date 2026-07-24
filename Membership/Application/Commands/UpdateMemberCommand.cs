using asistenciaBack.Membership.Application;
using asistenciaBack.Membership.Application.Dtos;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Membership.Application.Commands;

public class UpdateMemberCommand
{
    private readonly IMemberRepository _members;

    public UpdateMemberCommand(IMemberRepository members)
    {
        _members = members;
    }

    public async Task<Result<MemberDto>> ExecuteAsync(
        int id,
        UpdateMemberRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = CreateMemberCommand.Validate(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Address);

        if (validationError is not null)
        {
            return Result.Failure<MemberDto>(validationError);
        }

        var member = await _members.GetByIdAsync(id, cancellationToken);
        if (member is null || !member.IsActive)
        {
            return Result.Failure<MemberDto>("Member not found.");
        }

        member.Update(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Address);

        await _members.SaveChangesAsync(cancellationToken);

        return Result.Success(member.ToDto());
    }
}
