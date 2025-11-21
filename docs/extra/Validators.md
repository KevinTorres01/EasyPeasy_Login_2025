Tienes razón, en una **arquitectura N-Layered tradicional** el enfoque es más simple y directo:

## Arquitectura N-Layered típica:

```
┌─────────────────────┐
│   Presentation      │ (API/Controllers)
├─────────────────────┤
│   Application       │ (Services/Business Logic)
├─────────────────────┤
│   Domain            │ (Entities/Value Objects)
├─────────────────────┤
│   Infrastructure    │ (Data Access/Repositories)
└─────────────────────┘
```

## Validación en N-Layered:

### 1. **Validación en Controllers (Presentation)**
```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validación básica con ModelState (DataAnnotations)
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(
            request.Username, 
            request.Password, 
            request.IpAddress, 
            request.MacAddress);

        return result.IsSuccess 
            ? Ok(result.Data) 
            : Unauthorized(result.Error);
    }
}
```

### 2. **DTOs con DataAnnotations**
```csharp
using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string IpAddress { get; set; } = string.Empty;

    [Required]
    public string MacAddress { get; set; } = string.Empty;
}
```

### 3. **Validación en Services (Application)**
```csharp
using EasyPeasy_Login.Domain.ValueObjects;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public async Task<Result<SessionDto>> LoginAsync(
        string username, 
        string password, 
        string ipAddress, 
        string macAddress)
    {
        try
        {
            // Validación usando Value Objects
            var credentials = Credentials.Create(username, password);
            var ip = IpAddress.Create(ipAddress);
            var mac = MacAddress.Create(macAddress);

            // Lógica de negocio
            var user = await _userRepository.GetByUsernameAsync(credentials.Username);
            
            if (user == null)
                return Result<SessionDto>.Failure("Invalid credentials");

            // Verificar password, crear sesión, etc.
            // ...

            return Result<SessionDto>.Success(sessionDto);
        }
        catch (ArgumentException ex)
        {
            return Result<SessionDto>.Failure(ex.Message);
        }
    }
}
```

### 4. **Validación en Value Objects (Domain)**
```csharp
// Ya lo tienes implementado ✅
// Los Value Objects lanzan excepciones en su método Create()
```

## Opción con FluentValidation (Más robusto):

```csharp
using FluentValidation;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 50);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.IpAddress)
            .NotEmpty()
            .Must(BeValidIp).WithMessage("Invalid IP address");

        RuleFor(x => x.MacAddress)
            .NotEmpty()
            .Must(BeValidMac).WithMessage("Invalid MAC address");
    }

    private bool BeValidIp(string ip)
    {
        try { IpAddress.Create(ip); return true; }
        catch { return false; }
    }

    private bool BeValidMac(string mac)
    {
        try { MacAddress.Create(mac); return true; }
        catch { return false; }
    }
}
```

```csharp
// En el Controller
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var validator = new LoginRequestValidator();
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
        return BadRequest(validationResult.Errors);

    var result = await _authService.LoginAsync(/*...*/);
    return result.IsSuccess ? Ok(result.Data) : Unauthorized(result.Error);
}
```

## Resumen para N-Layered:

✅ **DataAnnotations** en DTOs para validación básica  
✅ **Value Objects** para reglas de dominio  
✅ **FluentValidation** (opcional) para validaciones complejas  
✅ **Try-catch** en Services para manejar excepciones de Value Objects  

**Más simple que Clean Architecture**, sin necesidad de MediatR ni Pipeline Behaviors.
