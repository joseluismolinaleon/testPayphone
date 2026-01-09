
using CustomerValidationSystem.Domain.Abstractions;
using CustomerValidationSystem.Domain.Entities;

namespace CustomerValidationSystem.Infrastructure.Repositories;

public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users;
    private int _nextId = 6; // Próximo ID disponible (después de los 5 seeds)

    public FakeUserRepository()
    {
        // Inicializar con 5 usuarios seed
        _users = new List<User>
        {
            User.Create(1, "Jose Molina", "0104826441"),
            User.Create(2, "Maria Lopez", "0987654324"),
            User.Create(3, "Carlos Ruiz", "0945678127"),
            User.Create(4, "Ana Torres", "0123456784"),
            User.Create(5, "Luis Gomez", "0912345672")
        };
    }

    /// <summary>
    /// Obtiene todos los usuarios del repositorio.
    /// </summary>
    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Simular operación async con Task.FromResult
        return await Task.FromResult(_users.AsEnumerable());
    }

    /// <summary>
    /// Busca un usuario por su Id.
    /// </summary>
    /// <returns>Usuario encontrado o null si no existe</returns>
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return await Task.FromResult(user);
    }

    /// <summary>
    /// Agrega un nuevo usuario al repositorio.
    /// Asigna automáticamente un Id único auto-incremental.
    /// </summary>
    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        // Asignar Id auto-incremental
        user.SetId(_nextId++);

        // Agregar a la lista en memoria
        _users.Add(user);

        return await Task.FromResult(user);
    }
}
