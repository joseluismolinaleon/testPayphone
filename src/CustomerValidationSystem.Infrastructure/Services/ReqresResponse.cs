
namespace CustomerValidationSystem.Infrastructure.Services;
public class ReqresResponse
{
    /// <summary>
    /// ID retornado por Reqres - Este valor representa el score crediticio.
    /// </summary>
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
