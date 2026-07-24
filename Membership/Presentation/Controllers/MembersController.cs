using asistenciaBack.Membership.Application.Commands;
using asistenciaBack.Membership.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asistenciaBack.Membership.Presentation.Controllers;

[ApiController]
[Route("api/members")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly GetMembersCommand _getMembers;
    private readonly GetMemberByIdCommand _getMemberById;
    private readonly CreateMemberCommand _createMember;
    private readonly UpdateMemberCommand _updateMember;
    private readonly DeactivateMemberCommand _deactivateMember;
    private readonly ActivateMemberCommand _activateMember;

    public MembersController(
        GetMembersCommand getMembers,
        GetMemberByIdCommand getMemberById,
        CreateMemberCommand createMember,
        UpdateMemberCommand updateMember,
        DeactivateMemberCommand deactivateMember,
        ActivateMemberCommand activateMember)
    {
        _getMembers = getMembers;
        _getMemberById = getMemberById;
        _createMember = createMember;
        _updateMember = updateMember;
        _deactivateMember = deactivateMember;
        _activateMember = activateMember;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getMembers.ExecuteAsync(cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getMemberById.ExecuteAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateMemberRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createMember.ExecuteAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateMemberRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateMember.ExecuteAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error == "Member not found."
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>Desactiva el miembro (soft delete). IsActive = false.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var result = await _deactivateMember.ExecuteAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error == "Member not found."
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>Reactiva un miembro desactivado. IsActive = true.</summary>
    [HttpPost("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        var result = await _activateMember.ExecuteAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error == "Member not found."
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }
}
