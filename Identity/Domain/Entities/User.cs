using asistenciaBack.Identity.Domain.Enums;

namespace asistenciaBack.Identity.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private User()
    {
    }

    public static User Create(string fullName, string email, string passwordHash, UserRole role)
    {
        return new User
        {
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
