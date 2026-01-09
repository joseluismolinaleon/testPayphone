
using CustomerValidationSystem.Domain.Entities;

namespace CustomerValidationSystem.Application.Users.DTOs;
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Mapea una entidad User a UserDto.
    /// </summary>
    public static UserDto FromEntity(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Job = user.Job,
            CreatedAt = user.CreatedAt
        };
    }
}
