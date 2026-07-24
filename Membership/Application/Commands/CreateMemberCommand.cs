using asistenciaBack.Membership.Application;
using asistenciaBack.Membership.Application.Dtos;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Membership.Domain.Entities;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Membership.Application.Commands;

public class CreateMemberCommand
{
    private readonly IMemberRepository _members;

    public CreateMemberCommand(IMemberRepository members)
    {
        _members = members;
    }

    public async Task<Result<MemberDto>> ExecuteAsync(
        CreateMemberRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request.FirstName, request.LastName, request.PhoneNumber, request.Address);
        if (validationError is not null)
        {
            return Result.Failure<MemberDto>(validationError);
        }

        var member = Member.Create(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Address);

        await _members.AddAsync(member, cancellationToken);
        await _members.SaveChangesAsync(cancellationToken);

        return Result.Success(member.ToDto());
    }

    internal static string? Validate(string firstName, string lastName, string phoneNumber, string? address)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return "First name is required.";
        }

        if (firstName.Trim().Length > 50)
        {
            return "First name must be at most 50 characters.";
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return "Last name is required.";
        }

        if (lastName.Trim().Length > 50)
        {
            return "Last name must be at most 50 characters.";
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return "Phone number is required.";
        }

        if (phoneNumber.Trim().Length > 20)
        {
            return "Phone number must be at most 20 characters.";
        }

        if (address is not null && address.Trim().Length > 150)
        {
            return "Address must be at most 150 characters.";
        }

        return null;
    }
}
