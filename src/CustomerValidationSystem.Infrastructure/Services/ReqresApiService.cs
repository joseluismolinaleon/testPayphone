

using System.Net.Http.Json;
using CustomerValidationSystem.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace CustomerValidationSystem.Infrastructure.Services;


//implementracion para llamada a servicio externo
public class ReqresApiService : IReqresApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReqresApiService> _logger;

    public ReqresApiService(HttpClient httpClient, ILogger<ReqresApiService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene el score crediticio llamando a la API externa.
    /// El score se extrae del campo "id" en la respuesta.
    /// </summary>
    /// <param name="nombre">Nombre completo de la persona</param>
    /// <param name="cedula">Cédula o identificación (se envía como "job")</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Score crediticio entre 300-850</returns>
    /// <exception cref="HttpRequestException">Si la API falla o no responde</exception>
    public async Task<int> GetCreditScoreAsync(
        string nombre,
        string cedula,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Calling Reqres API for credit score. Name: {Name}, Cedula: {Cedula}",
                nombre, cedula);

            // Preparar request body
            var requestBody = new ReqresRequest
            {
                Name = nombre,
                Job = cedula  // Cédula se envía como "job"
            };

            // POST /api/users con JSON body
            var response = await _httpClient.PostAsJsonAsync(
                "/api/users",
                requestBody,
                cancellationToken);

            // Validar status code 2xx
            response.EnsureSuccessStatusCode();

            // Deserializar respuesta
            var result = await response.Content.ReadFromJsonAsync<ReqresResponse>(cancellationToken);

            if (result == null)
            {
                _logger.LogError("Reqres API returned null response for Cedula: {Cedula}", cedula);
                throw new HttpRequestException("API returned empty response");
            }

            // El score viene en el campo "id"
            var score = result.Id;

            _logger.LogInformation(
                "Successfully received score {Score} for Cedula: {Cedula}",
                score, cedula);

            return score;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "HTTP error calling Reqres API for Cedula: {Cedula}. Message: {Message}",
                cedula, ex.Message);
            throw;  // Re-lanzar para que CommandHandler maneje con TransactionStatus.Error
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex,
                "Timeout calling Reqres API for Cedula: {Cedula}",
                cedula);
            throw new HttpRequestException("API request timed out", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error calling Reqres API for Cedula: {Cedula}",
                cedula);
            throw new HttpRequestException($"Unexpected error: {ex.Message}", ex);
        }
    }

}
