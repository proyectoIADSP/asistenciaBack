using System.Text.RegularExpressions;
using asistenciaBack.Membership.Application;
using asistenciaBack.Membership.Application.Dtos;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Membership.Domain.Entities;
using asistenciaBack.Shared.Results;

namespace asistenciaBack.Membership.Application.Commands;

public class CreateMemberCommand
{
    private static readonly Regex NineDigitPhone = new(@"^\d{9}$", RegexOptions.Compiled);

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

        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var phone = request.PhoneNumber.Trim();

        if (await _members.ExistsByFullNameAsync(firstName, lastName, cancellationToken))
        {
            return Result.Failure<MemberDto>("Ese miembro ya está registrado con ese nombre y apellido.");
        }

        if (await _members.ExistsByPhoneNumberAsync(phone, cancellationToken))
        {
            return Result.Failure<MemberDto>("Ese número de celular ya está registrado.");
        }

        var member = Member.Create(firstName, lastName, phone, request.Address);

        await _members.AddAsync(member, cancellationToken);
        await _members.SaveChangesAsync(cancellationToken);

        return Result.Success(member.ToDto());
    }

    internal static string? Validate(string firstName, string lastName, string phoneNumber, string? address)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return "El nombre es obligatorio.";
        }

        if (firstName.Trim().Length > 50)
        {
            return "El nombre debe tener máximo 50 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return "El apellido es obligatorio.";
        }

        if (lastName.Trim().Length > 50)
        {
            return "El apellido debe tener máximo 50 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return "El número de celular es obligatorio.";
        }

        if (!NineDigitPhone.IsMatch(phoneNumber.Trim()))
        {
            return "El número de celular debe tener exactamente 9 dígitos.";
        }

        if (address is not null && address.Trim().Length > 150)
        {
            return "La dirección debe tener máximo 150 caracteres.";
        }

        return null;
    }
}
