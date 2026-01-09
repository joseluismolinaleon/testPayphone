

namespace CustomerValidationSystem.Domain.Abstractions;
public interface IReqresApiService
{
    /// <summary>
    /// Obtiene el score crediticio de una persona desde la API externa. // Para llamada a api externa
    /// </summary>
    /// <param name="nombre">Nombre completo de la persona</param>
    /// <param name="cedula">Cédula o identificación de la persona</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Score crediticio entre 300-850</returns>
    /// <exception cref="HttpRequestException">
    /// Si la API externa no responde o retorna error.
    /// El caller debe manejar esta excepción y asignar TransactionStatus.Error.
    /// </exception>
    public Task<int> GetCreditScoreAsync(
        string nombre,
        string cedula,
        CancellationToken cancellationToken = default);
}
