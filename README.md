# Arquitectura del Sistema - CustomerValidationSystem

## 📋 Resumen Ejecutivo

Sistema de validación de clientes con scoring crediticio que consume API externa (Reqres) y aplica reglas de negocio para aprobar/rechazar transacciones. Implementado con **Clean Architecture + CQRS + FakeRepository** para entrega rápida y demo funcional en 2h45.

---

## 🏗️ Arquitectura Propuesta

### Diagrama de Capas

```
┌─────────────────────────────────────────────────────────┐
│                   API Layer (REST)                       │
│  Controllers: UsersController, TransactionsController    │
│  Middleware: Exception handling, Logging                 │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│              Application Layer (CQRS)                    │
│  Commands: CreateUserCommand, CreateTransactionCommand  │
│  Queries: GetAllUsersQuery, GetTransactionsByUserId     │
│  Handlers: Business logic + Validation                  │
│  DTOs: UserDto, TransactionDto                          │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│                 Domain Layer                             │
│  Entities: User, Transaction (con Factory Methods)      │
│  Enums: TransactionStatus (Approved, Rejected)          │
│  Abstractions: IUserRepository, ITransactionRepository  │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│            Infrastructure Layer                          │
│  Repositories: FakeUserRepository, FakeTransactionRepo  │
│  Services: ReqresApiService (HttpClient)                │
│  Data: In-Memory List<T> (sin EF Core)                  │
└──────────────────────────────────────────────────────────┘
```

---

## 🎯 Decisiones Arquitectónicas

### 1. Clean Architecture (4 Capas)

**Decisión:** Separar en Domain → Application → Infrastructure → API

**Justificación:**
- ✅ **Testeable:** Domain y Application sin dependencias externas
- ✅ **Mantenible:** Cambios en UI o DB no afectan lógica de negocio
- ✅ **Escalable:** Puedo agregar nuevas features sin romper existentes
- ✅ **Demuestra conocimiento:** Para líder técnico es clave mostrar arquitectura sólida

**Trade-off:**
- ❌ Más archivos/carpetas que un proyecto monolítico
- ✅ Pero compensa con claridad y profesionalismo

### 2. FakeRepository (In-Memory)

**Decisión:** Usar `List<T>` en memoria en lugar de Entity Framework + SQL Server

**Justificación CLAVE:**
```csharp
// FakeUserRepository.cs
private static readonly List<User> _users = new();
private static int _nextId = 1;

public Task<User> AddAsync(User user, CancellationToken ct)
{
    user.SetId(_nextId++);
    _users.Add(user);
    return Task.FromResult(user);
}
```

**Razones de negocio:**
1. 🎯 **Foco en negocio:** Dedicar esfuerzo en lógica de scoring, no en SQL
2. ✅ **Demo funcional:** `dotnet run` y funciona sin configurar BD
3. 🧪 **Tests rápidos:** Tests unitarios sin BD ni containers
4. 📦 **Zero config:** No requiere LocalDB, Docker, ni connection strings
5. 🔄 **Evolutivo:** Cambiar a EF Core después es trivial (misma interfaz)

**Trade-off:**
- ❌ Datos volátiles (se pierden al reiniciar)
- ✅ Pero para prueba técnica es perfecto
- ✅ Cambiar a EF Core después es trivial (misma interfaz)

### 3. CQRS con MediatR

**Decisión:** Commands (Create) y Queries (GetAll, GetById) separados

**Justificación:**
```csharp
// Command: Modifica estado
public record CreateTransactionCommand(int UserId, decimal Amount) 
    : IRequest<TransactionDto>;

// Query: Solo lectura
public record GetTransactionsByUserIdQuery(int UserId) 
    : IRequest<IEnumerable<TransactionDto>>;
```

**Beneficios:**
- ✅ **SRP:** Cada handler tiene una responsabilidad
- ✅ **Testeable:** Mock IMediator fácilmente
- ✅ **Pipeline:** FluentValidation se integra automáticamente
- ✅ **Escalable:** Agregar nuevo endpoint = 1 command + 1 handler

### 4. Validación con FluentValidation

**Decisión:** Validar en handlers con FluentValidation 11.11.0

**Código:**
```csharp
public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(999999.99m);
    }
}
```

**Beneficios:**
- ✅ **Declarativa:** Reglas legibles
- ✅ **Reusable:** Misma validación en tests
- ✅ **Error handling:** ASP.NET retorna 400 automáticamente

### 5. Manejo de Errores

**Estrategia:**
```csharp
public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken ct)
{
    // 1. Verificar usuario existe
    var user = await _userRepository.GetByIdAsync(request.UserId, ct);
    if (user == null) throw new NotFoundException($"User {request.UserId} not found");

    // 2. Llamar API con try-catch
    try
    {
        score = await _reqresApiService.GetCreditScoreAsync(user.Name, user.Job, ct);
        status = DetermineTransactionStatus(score, request.Amount);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "API failed for User {UserId}", user.Id);
        score = 0;
        status = TransactionStatus.Rejected; // Rechazar por seguridad
    }

    // 3. Guardar transacción con status
    var transaction = Transaction.Create(user.Id, request.Amount, score, status);
    await _transactionRepository.AddAsync(transaction, ct);
    
    return TransactionDto.FromEntity(transaction);
}
```

**Capas de error:**
1. **Validación:** FluentValidation → 400 Bad Request
2. **NotFound:** User no existe → 404 Not Found
3. **API Externa:** HttpClient falla → Reject transaction + 201 Created con status=Rejected
4. **Global:** Middleware captura excepciones no manejadas

### 6. Consumo de API Externa

**Implementación:**
```csharp
public class ReqresApiService : IReqresApiService
{
    private readonly HttpClient _httpClient;

    public async Task<int> GetCreditScoreAsync(string name, string job, CancellationToken ct)
    {
        var request = new ReqresRequest { Name = name, Job = job };
        
        var response = await _httpClient.PostAsJsonAsync("/api/users", request, ct);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ReqresResponse>(ct);
        return result?.Id ?? 0; // Score = campo "id" de Reqres
    }
}
```

**HttpClient configurado en DI:**
```csharp
services.AddHttpClient<IReqresApiService, ReqresApiService>(client =>
{
    client.BaseAddress = new Uri(configuration["ReqresApi:BaseUrl"]);
    client.DefaultRequestHeaders.Add("x-api-key", configuration["ReqresApi:ApiKey"]);
    client.Timeout = TimeSpan.FromSeconds(10);
});
```

### 7. Logging Estructurado (Serilog)

**Configuración:**
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(telemetryConfiguration)
    .WriteTo.Stackify()
    .Enrich.FromLogContext()
    .CreateLogger();
```

**Logs clave:**
```csharp
_logger.LogInformation("Creating transaction for User {UserId}, Amount: ${Amount}", 
    user.Id, request.Amount);
_logger.LogInformation("Credit score retrieved: {Score} for User {UserId}", 
    score, user.Id);
_logger.LogError(ex, "Failed to get credit score from API for User {UserId}", 
    user.Id);
```

---

## 🔄 Flujo Completo de Transacción

### Diagrama de Secuencia

```
Cliente                Controller             Handler                  Repository         API Externa
  │                        │                      │                         │                  │
  │  POST /transactions    │                      │                         │                  │
  │  {userId:1, amount:500}│                      │                         │                  │
  ├───────────────────────►│                      │                         │                  │
  │                        │  Send(Command)       │                         │                  │
  │                        ├─────────────────────►│                         │                  │
  │                        │                      │  GetByIdAsync(1)        │                  │
  │                        │                      ├────────────────────────►│                  │
  │                        │                      │  User{Name,Job}         │                  │
  │                        │                      │◄────────────────────────┤                  │
  │                        │                      │                         │                  │
  │                        │                      │  GetCreditScoreAsync(Name,Job)             │
  │                        │                      ├────────────────────────────────────────────►│
  │                        │                      │                         │  POST /api/users │
  │                        │                      │                         │  {name,job}      │
  │                        │                      │  Score (id=450)         │                  │
  │                        │                      │◄────────────────────────────────────────────┤
  │                        │                      │                         │                  │
  │                        │                      │  DetermineStatus(450,500)                  │
  │                        │                      │  → Rejected (score<500) │                  │
  │                        │                      │                         │                  │
  │                        │                      │  AddAsync(Transaction)  │                  │
  │                        │                      ├────────────────────────►│                  │
  │                        │                      │  Transaction{Id:1}      │                  │
  │                        │                      │◄────────────────────────┤                  │
  │                        │  TransactionDto      │                         │                  │
  │                        │◄─────────────────────┤                         │                  │
  │  201 Created           │                      │                         │                  │
  │  {id:1,score:450,      │                      │                         │                  │
  │   status:"Rejected"}   │                      │                         │                  │
  │◄───────────────────────┤                      │                         │                  │
```

### Código del Flujo

```csharp
// 1. Request HTTP
POST /api/v1/transactions
{
  "userId": 1,
  "amount": 500.00
}

// 2. Controller → Command
var command = new CreateTransactionCommand(1, 500);
var transaction = await _mediator.Send(command);

// 3. Handler → Validación
FluentValidation valida: userId > 0, amount > 0

// 4. Handler → Buscar Usuario
var user = await _userRepository.GetByIdAsync(1);
// user = { Id:1, Name:"Jose Molina", Job:"0104826441" }

// 5. Handler → Consultar Score
score = await _reqresApiService.GetCreditScoreAsync("Jose Molina", "0104826441");
// API Reqres retorna: { "id": 450, ... } → score = 450

// 6. Handler → Aplicar Reglas
status = DetermineTransactionStatus(450, 500);
// Score < 500 → status = Rejected

// 7. Handler → Guardar
var transaction = Transaction.Create(1, 500, 450, TransactionStatus.Rejected);
await _transactionRepository.AddAsync(transaction);

// 8. Controller → Response
return CreatedAtAction(nameof(GetById), new { id = 1 }, transactionDto);

// Response HTTP 201 Created
{
  "id": 1,
  "userId": 1,
  "amount": 500.00,
  "score": 450,
  "status": "Rejected",
  "createdAt": "2026-01-09T12:00:00Z"
}
```

---

## 📊 Reglas de Negocio Implementadas

```csharp
private TransactionStatus DetermineTransactionStatus(int score, decimal amount)
{
    if (score >= 700) return TransactionStatus.Approved;
    if (score < 500) return TransactionStatus.Rejected;
    
    // Score 500-699
    return amount < 1000 ? TransactionStatus.Approved : TransactionStatus.Rejected;
}
```

**Tabla de Decisión:**

| Score | Amount | Status | Razón |
|-------|--------|--------|-------|
| 750 | $500 | ✅ Approved | Score ≥ 700 |
| 720 | $5,000 | ✅ Approved | Score ≥ 700 |
| 620 | $800 | ✅ Approved | 500-699 Y < $1,000 |
| 580 | $1,500 | ❌ Rejected | 500-699 Y ≥ $1,000 |
| 450 | $100 | ❌ Rejected | Score < 500 |
| 0 | $500 | ❌ Rejected | API falló |

---

## 🧪 Testing

### Cobertura de Tests Unitarios

```csharp
// 1. Scoring Rules Tests (8 tests)
[Theory]
[InlineData(750, 500, TransactionStatus.Approved)]    // High score
[InlineData(450, 500, TransactionStatus.Rejected)]    // Low score
[InlineData(620, 800, TransactionStatus.Approved)]    // Medium score + low amount
[InlineData(580, 1500, TransactionStatus.Rejected)]   // Medium score + high amount
public async Task CreateTransaction_AppliesCorrectBusinessRules(
    int mockScore, decimal amount, TransactionStatus expectedStatus)
{
    // Arrange: Mock API to return specific score
    // Act: Create transaction
    // Assert: Verify status matches business rules
}

// 2. Validation Tests (5 tests)
[Fact]
public void Validator_InvalidUserId_FailsValidation()
{
    var command = new CreateTransactionCommand(0, 500);
    var result = validator.Validate(command);
    Assert.False(result.IsValid);
}

// 3. API Failure Tests (2 tests)
[Fact]
public async Task CreateTransaction_ApiFailure_ReturnsRejected()
{
    // Arrange: Mock API to throw HttpRequestException
    // Act: Create transaction
    // Assert: Status = Rejected, Score = 0
}

// 4. Relationship Tests (3 tests)
[Fact]
public async Task User_CanHaveMultipleTransactions()
{
    // Test 1-N relationship
}
```

**Comando:**
```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## 📦 Tecnologías y Librerías

### NuGet Packages

```xml
<!-- Application Layer -->
<PackageReference Include="MediatR" Version="12.5.0" />
<PackageReference Include="FluentValidation" Version="11.11.0" />

<!-- Infrastructure Layer -->
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.ApplicationInsights" Version="4.0.0" />
<PackageReference Include="Stackify.Api" Version="1.0.0" />

<!-- API Layer -->
<PackageReference Include="Asp.Versioning.Mvc.ApiExplorer" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="Azure.Extensions.AspNetCore.Configuration.Secrets" Version="1.3.0" />

<!-- Tests -->
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="xunit" Version="2.6.2" />
```

### Justificación de Librerías

| Librería | Justificación |
|----------|---------------|
| **MediatR** | CQRS pattern, desacopla controllers de handlers |
| **FluentValidation** | Validación declarativa y testeable |
| **Serilog** | Logging estructurado para producción |
| **Moq** | Mock de dependencias en tests |
| **Asp.Versioning** | Versionado de API (v1, v2) |
| **Azure Key Vault** | Secrets management en Azure |

---

## 🎯 Diferenciadores Implementados

1. ✅ **Clean Architecture** (Domain-Application-Infrastructure-API)
2. ✅ **CQRS** (Commands/Queries separados)
3. ✅ **Factory Methods** (User.Create, Transaction.Create)
4. ✅ **FakeRepository** (demo sin BD en 5 minutos)
5. ✅ **API Versioning** (v1/users, v1/transactions)
6. ✅ **Logging estructurado** (Serilog con 3 sinks)
7. ✅ **Exception handling** (NotFoundException, API failures)
8. ✅ **18 Unit Tests** (Moq + FluentAssertions)
9. ✅ **Swagger UI** (documentación automática)
10. ✅ **Application Insights** (telemetría en Azure)
11. ✅ **Health Checks** (/healthz endpoint)
12. ✅ **Localization** (i18n para en-US y es-ES)

---

## 📌 Conclusión

Esta arquitectura demuestra:

1. ✅ **Conocimiento sólido** de Clean Architecture y SOLID
2. ✅ **Pragmatismo** usando FakeRepository para demo rápido
3. ✅ **Calidad** con 18 tests unitarios y logging estructurado
4. ✅ **Production-ready** con telemetría, health checks, y error handling
5. ✅ **Escalabilidad** fácil migrar a EF Core cuando sea necesario

**Justificación de FakeRepository:**
> "Preferí invertir esfuerzo extra en lógica de negocio robusta, tests completos, y features de producción (Serilog, Application Insights, Health Checks) en lugar de configurar Entity Framework + SQL Server. El código está diseñado con Clean Architecture, por lo que cambiar a EF Core es trivial: solo reemplazar FakeRepository por EF Repository manteniendo la misma interfaz IUserRepository."

---

**Desarrollado por:** Jose Molina  
**Fecha:** 9 de Enero, 2026  
**Framework:** .NET 8.0  
**Patrón:** Clean Architecture + CQRS + FakeRepository
