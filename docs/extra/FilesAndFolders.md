# Estructura de Proyectos para Portal Cautivo

## **Solución Multi-Proyecto**

```
EasyPeasy_Login/
├── EasyPeasy_Login.sln
├── README.md
├── .gitignore
│
├── src/
│   ├── EasyPeasy_Login.Web/                          # Aplicación Blazor Server
│   │   ├── EasyPeasy_Login.Web.csproj
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   │
│   │   ├── Pages/
│   │   │   ├── Portal/                             # UI para clientes
│   │   │   │   ├── _Host.cshtml
│   │   │   │   ├── Login.razor
│   │   │   │   ├── Login.razor.cs
│   │   │   │   ├── Terms.razor
│   │   │   │   ├── Terms.razor.cs
│   │   │   │   ├── Success.razor
│   │   │   │   └── Error.razor
│   │   │   │
│   │   │   └── Admin/                              # UI para administrador
│   │   │       ├── _AdminHost.cshtml
│   │   │       ├── Dashboard.razor
│   │   │       ├── Dashboard.razor.cs
│   │   │       ├── Users/
│   │   │       │   ├── Index.razor
│   │   │       │   ├── Create.razor
│   │   │       │   ├── Edit.razor
│   │   │       │   └── Delete.razor
│   │   │       ├── Devices/
│   │   │       │   ├── Index.razor
│   │   │       │   └── Details.razor
│   │   │       └── Settings/
│   │   │           └── Index.razor
│   │   │
│   │   ├── Components/                             # Componentes reutilizables
│   │   │   ├── Shared/
│   │   │   │   ├── MainLayout.razor
│   │   │   │   ├── AdminLayout.razor
│   │   │   │   ├── NavMenu.razor
│   │   │   │   └── LoadingSpinner.razor
│   │   │   ├── Portal/
│   │   │   │   ├── AnimatedAvatar.razor
│   │   │   │   ├── LoginForm.razor
│   │   │   │   └── TermsContent.razor
│   │   │   └── Admin/
│   │   │       ├── UserTable.razor
│   │   │       ├── DeviceTable.razor
│   │   │       ├── StatisticsCard.razor
│   │   │       └── SessionMonitor.razor
│   │   │
│   │   ├── Middleware/
│   │   │   ├── RequestRouterMiddleware.cs
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── RequestLoggingMiddleware.cs
│   │   │
│   │   ├── Controllers/                            # API Controllers
│   │   │   ├── AuthController.cs
│   │   │   ├── DetectionController.cs
│   │   │   └── AdminApiController.cs
│   │   │
│   │   ├── HostedServices/
│   │   │   ├── CaptivePortalHostedService.cs
│   │   │   ├── SessionCleanupService.cs
│   │   │   └── NetworkMonitoringService.cs
│   │   │
│   │   ├── wwwroot/
│   │   │   ├── css/
│   │   │   │   ├── portal.css
│   │   │   │   ├── admin.css
│   │   │   │   └── shared.css
│   │   │   ├── js/
│   │   │   │   ├── portal.js
│   │   │   │   └── admin.js
│   │   │   ├── images/
│   │   │   │   ├── avatars/
│   │   │   │   └── logos/
│   │   │   └── lib/
│   │   │
│   │   └── Extensions/
│   │       ├── ServiceCollectionExtensions.cs
│   │       └── ApplicationBuilderExtensions.cs
│   │
│   ├── EasyPeasy_Login.Application/                  # Lógica de Negocio
│   │   ├── EasyPeasy_Login.Application.csproj
│   │   │
│   │   ├── Services/
│   │   │   ├── Authentication/
│   │   │   │   ├── IAuthenticationService.cs
│   │   │   │   ├── AuthenticationService.cs
│   │   │   │   └── AuthResult.cs
│   │   │   │
│   │   │   ├── UserManagement/
│   │   │   │   ├── IUserManagementService.cs
│   │   │   │   ├── UserManagementService.cs
│   │   │   │   └── UserValidator.cs
│   │   │   │
│   │   │   ├── DeviceManagement/
│   │   │   │   ├── IDeviceManagementService.cs
│   │   │   │   ├── DeviceManagementService.cs
│   │   │   │   └── DeviceTracker.cs
│   │   │   │
│   │   │   ├── SessionManagement/
│   │   │   │   ├── ISessionManagementService.cs
│   │   │   │   ├── SessionManagementService.cs
│   │   │   │   └── SessionValidator.cs
│   │   │   │
│   │   │   ├── NetworkControl/
│   │   │   │   ├── INetworkControlService.cs
│   │   │   │   ├── NetworkControlService.cs
│   │   │   │   └── NetworkValidator.cs
│   │   │   │
│   │   │   └── CaptiveDetection/
│   │   │       ├── ICaptivePortalDetectionService.cs
│   │   │       ├── CaptivePortalDetectionService.cs
│   │   │       └── DetectionEndpoints.cs
│   │   │
│   │   ├── DTOs/
│   │   │   ├── Authentication/
│   │   │   │   ├── LoginRequestDto.cs
│   │   │   │   ├── LoginResponseDto.cs
│   │   │   │   └── AuthResultDto.cs
│   │   │   │
│   │   │   ├── User/
│   │   │   │   ├── UserDto.cs
│   │   │   │   ├── CreateUserDto.cs
│   │   │   │   ├── UpdateUserDto.cs
│   │   │   │   └── UserListDto.cs
│   │   │   │
│   │   │   ├── Device/
│   │   │   │   ├── DeviceDto.cs
│   │   │   │   ├── DeviceInfoDto.cs
│   │   │   │   └── DeviceHistoryDto.cs
│   │   │   │
│   │   │   └── Session/
│   │   │       ├── SessionDto.cs
│   │   │       ├── SessionInfoDto.cs
│   │   │       └── ActiveSessionDto.cs
│   │   │
│   │   ├── Validators/
│   │   │   ├── UserDtoValidator.cs
│   │   │   ├── LoginRequestValidator.cs
│   │   │   └── DeviceDtoValidator.cs
│   │   │
│   │   ├── Mappings/
│   │   │   ├── UserMappingProfile.cs
│   │   │   ├── DeviceMappingProfile.cs
│   │   │   └── SessionMappingProfile.cs
│   │   │
│   │   └── Exceptions/
│   │       ├── AuthenticationException.cs
│   │       ├── UserNotFoundException.cs
│   │       ├── DeviceNotFoundException.cs
│   │       └── SessionExpiredException.cs
│   │
│   ├── EasyPeasy_Login.Domain/                       # Modelos de Dominio
│   │   ├── EasyPeasy_Login.Domain.csproj
│   │   │
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Device.cs
│   │   │   ├── Session.cs
│   │   │   ├── ConnectionHistory.cs
│   │   │   └── FirewallRule.cs
│   │   │
│   │   ├── ValueObjects/
│   │   │   ├── IpAddress.cs
│   │   │   ├── MacAddress.cs
│   │   │   ├── SessionToken.cs
│   │   │   └── Credentials.cs
│   │   │
│   │   ├── Enums/
│   │   │   ├── UserRole.cs
│   │   │   ├── DeviceStatus.cs
│   │   │   ├── SessionStatus.cs
│   │   │   └── OSType.cs
│   │   │
│   │   ├── Interfaces/
│   │   │   └── IAuditableEntity.cs
│   │   │
│   │   └── Common/
│   │       ├── BaseEntity.cs
│   │       └── Constants.cs
│   │
│   ├── EasyPeasy_Login.Infrastructure/               # Acceso a Datos e Infraestructura
│   │   ├── EasyPeasy_Login.Infrastructure.csproj
│   │   │
│   │   ├── Persistence/
│   │   │   ├── Repositories/
│   │   │   │   ├── IUserRepository.cs
│   │   │   │   ├── UserRepository.cs
│   │   │   │   ├── IDeviceRepository.cs
│   │   │   │   ├── DeviceRepository.cs
│   │   │   │   ├── ISessionRepository.cs
│   │   │   │   ├── SessionRepository.cs
│   │   │   │   └── BaseRepository.cs
│   │   │   │
│   │   │   ├── DataStore/
│   │   │   │   ├── IDataStore.cs
│   │   │   │   ├── JSONDataStore.cs
│   │   │   │   └── DataStoreOptions.cs
│   │   │   │
│   │   │   └── Configuration/
│   │   │       └── RepositoryConfiguration.cs
│   │   │
│   │   ├── Network/
│   │   │   ├── Firewall/
│   │   │   │   ├── IFirewallManager.cs
│   │   │   │   ├── IptablesManager.cs
│   │   │   │   ├── FirewallRule.cs
│   │   │   │   └── FirewallCommandBuilder.cs
│   │   │   │
│   │   │   ├── DNS/
│   │   │   │   ├── IDnsManager.cs
│   │   │   │   ├── DnsmasqManager.cs
│   │   │   │   ├── DHCPConfig.cs
│   │   │   │   ├── DNSConfig.cs
│   │   │   │   └── DnsmasqConfigGenerator.cs
│   │   │   │
│   │   │   └── NetworkInterfaces/
│   │   │       ├── INetworkInterfaceManager.cs
│   │   │       ├── NetworkInterfaceManager.cs
│   │   │       └── InterfaceConfiguration.cs
│   │   │
│   │   ├── System/
│   │   │   ├── ProcessExecutor/
│   │   │   │   ├── IProcessExecutor.cs
│   │   │   │   ├── ProcessExecutor.cs
│   │   │   │   └── ExecutionResult.cs
│   │   │   │
│   │   │   └── FileSystem/
│   │   │       ├── IFileSystemService.cs
│   │   │       └── FileSystemService.cs
│   │   │
│   │   ├── Security/
│   │   │   ├── Cryptography/
│   │   │   │   ├── IPasswordHasher.cs
│   │   │   │   ├── PasswordHasher.cs
│   │   │   │   └── HashingOptions.cs
│   │   │   │
│   │   │   └── TokenGeneration/
│   │   │       ├── ITokenGenerator.cs
│   │   │       └── TokenGenerator.cs
│   │   │
│   │   └── Extensions/
│   │       └── InfrastructureServiceExtensions.cs
│   │
│   └── EasyPeasy_Login.Shared/                       # Código compartido
│       ├── EasyPeasy_Login.Shared.csproj
│       │
│       ├── Constants/
│       │   ├── ApiRoutes.cs
│       │   ├── ConfigurationKeys.cs
│       │   ├── ErrorMessages.cs
│       │   └── NetworkConstants.cs
│       │
│       ├── Helpers/
│       │   ├── NetworkHelper.cs
│       │   ├── ValidationHelper.cs
│       │   └── StringHelper.cs
│       │
│       └── Extensions/
│           ├── StringExtensions.cs
│           ├── DateTimeExtensions.cs
│           └── CollectionExtensions.cs
│
├── tests/
│   ├── EasyPeasy_Login.UnitTests/
│   │   ├── EasyPeasy_Login.UnitTests.csproj
│   │   │
│   │   ├── Application/
│   │   │   ├── Services/
│   │   │   │   ├── AuthenticationServiceTests.cs
│   │   │   │   ├── UserManagementServiceTests.cs
│   │   │   │   ├── DeviceManagementServiceTests.cs
│   │   │   │   └── SessionManagementServiceTests.cs
│   │   │   │
│   │   │   └── Validators/
│   │   │       └── UserDtoValidatorTests.cs
│   │   │
│   │   ├── Domain/
│   │   │   ├── Entities/
│   │   │   │   ├── UserTests.cs
│   │   │   │   ├── DeviceTests.cs
│   │   │   │   └── SessionTests.cs
│   │   │   │
│   │   │   └── ValueObjects/
│   │   │       ├── IpAddressTests.cs
│   │   │       └── MacAddressTests.cs
│   │   │
│   │   └── Infrastructure/
│   │       ├── Repositories/
│   │       │   ├── UserRepositoryTests.cs
│   │       │   └── DeviceRepositoryTests.cs
│   │       │
│   │       └── Network/
│   │           ├── IptablesManagerTests.cs
│   │           └── DnsmasqManagerTests.cs
│   │
│   ├── EasyPeasy_Login.IntegrationTests/
│   │   ├── EasyPeasy_Login.IntegrationTests.csproj
│   │   │
│   │   ├── API/
│   │   │   ├── AuthControllerTests.cs
│   │   │   └── DetectionControllerTests.cs
│   │   │
│   │   ├── Services/
│   │   │   └── NetworkControlServiceTests.cs
│   │   │
│   │   └── Infrastructure/
│   │       └── DataStoreTests.cs
│   │
│   └── EasyPeasy_Login.E2ETests/
│       ├── EasyPeasy_Login.E2ETests.csproj
│       │
│       ├── Portal/
│       │   └── LoginFlowTests.cs
│       │
│       └── Admin/
│           └── UserManagementFlowTests.cs
│
├── scripts/
│   ├── setup.sh                                    # Instalación inicial
│   ├── install-dependencies.sh                     # Dependencias del sistema
│   ├── create-certificates.sh                      # Certificados SSL
│   ├── configure-network.sh                        # Configuración de red
│   ├── cleanup.sh                                  # Limpieza de configuración
│   └── run-dev.sh                                  # Ejecutar en desarrollo
│
├── config/
│   ├── dnsmasq/
│   │   ├── dnsmasq.conf.template
│   │   └── dhcp-hosts.conf
│   │
│   ├── iptables/
│   │   ├── rules.v4.template
│   │   └── restore-rules.sh
│   │
│   └── certificates/
│       └── README.md
│
├── data/                                           # Datos en runtime (gitignored)
│   ├── users.json
│   ├── devices.json
│   ├── sessions.json
│   └── logs/
│
└── docs/
    ├── architecture.md
    ├── api-documentation.md
    ├── deployment.md
    ├── network-configuration.md
    └── troubleshooting.md
```

---

## **Archivos de Proyecto (.csproj)**

### **EasyPeasy_Login.Web.csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="8.0.*" />
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.*" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.*" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\EasyPeasy_Login.Application\EasyPeasy_Login.Application.csproj" />
    <ProjectReference Include="..\EasyPeasy_Login.Infrastructure\EasyPeasy_Login.Infrastructure.csproj" />
  </ItemGroup>

</Project>
```

### **EasyPeasy_Login.Application.csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="12.0.*" />
    <PackageReference Include="FluentValidation" Version="11.9.*" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.*" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\EasyPeasy_Login.Domain\EasyPeasy_Login.Domain.csproj" />
  </ItemGroup>

</Project>
```

### **EasyPeasy_Login.Domain.csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

### **EasyPeasy_Login.Infrastructure.csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.Text.Json" Version="8.0.*" />
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.*" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="8.0.*" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\EasyPeasy_Login.Domain\EasyPeasy_Login.Domain.csproj" />
  </ItemGroup>

</Project>
```

### **EasyPeasy_Login.Shared.csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

---

## **Comandos para Crear la Estructura**

```bash
#!/bin/bash

# Crear solución
dotnet new sln -n EasyPeasy_Login

# Crear proyectos
dotnet new blazorserver -n EasyPeasy_Login.Web -o src/EasyPeasy_Login.Web
dotnet new classlib -n EasyPeasy_Login.Application -o src/EasyPeasy_Login.Application
dotnet new classlib -n EasyPeasy_Login.Domain -o src/EasyPeasy_Login.Domain
dotnet new classlib -n EasyPeasy_Login.Infrastructure -o src/EasyPeasy_Login.Infrastructure
dotnet new classlib -n EasyPeasy_Login.Shared -o src/EasyPeasy_Login.Shared

# Crear proyectos de pruebas
dotnet new xunit -n EasyPeasy_Login.UnitTests -o tests/EasyPeasy_Login.UnitTests
dotnet new xunit -n EasyPeasy_Login.IntegrationTests -o tests/EasyPeasy_Login.IntegrationTests
dotnet new xunit -n EasyPeasy_Login.E2ETests -o tests/EasyPeasy_Login.E2ETests

# Agregar proyectos a la solución
dotnet sln add src/EasyPeasy_Login.Web/EasyPeasy_Login.Web.csproj
dotnet sln add src/EasyPeasy_Login.Application/EasyPeasy_Login.Application.csproj
dotnet sln add src/EasyPeasy_Login.Domain/EasyPeasy_Login.Domain.csproj
dotnet sln add src/EasyPeasy_Login.Infrastructure/EasyPeasy_Login.Infrastructure.csproj
dotnet sln add src/EasyPeasy_Login.Shared/EasyPeasy_Login.Shared.csproj
dotnet sln add tests/EasyPeasy_Login.UnitTests/EasyPeasy_Login.UnitTests.csproj
dotnet sln add tests/EasyPeasy_Login.IntegrationTests/EasyPeasy_Login.IntegrationTests.csproj
dotnet sln add tests/EasyPeasy_Login.E2ETests/EasyPeasy_Login.E2ETests.csproj

# Referencias entre proyectos
dotnet add src/EasyPeasy_Login.Web reference src/EasyPeasy_Login.Application
dotnet add src/EasyPeasy_Login.Web reference src/EasyPeasy_Login.Infrastructure
dotnet add src/EasyPeasy_Login.Application reference src/EasyPeasy_Login.Domain
dotnet add src/EasyPeasy_Login.Infrastructure reference src/EasyPeasy_Login.Domain

# Referencias de tests
dotnet add tests/EasyPeasy_Login.UnitTests reference src/EasyPeasy_Login.Application
dotnet add tests/EasyPeasy_Login.UnitTests reference src/EasyPeasy_Login.Domain
dotnet add tests/EasyPeasy_Login.UnitTests reference src/EasyPeasy_Login.Infrastructure
dotnet add tests/EasyPeasy_Login.IntegrationTests reference src/EasyPeasy_Login.Web
dotnet add tests/EasyPeasy_Login.E2ETests reference src/EasyPeasy_Login.Web

# Crear directorios adicionales
mkdir -p scripts config/{dnsmasq,iptables,certificates} data/logs docs
```

---

## **.gitignore**

```gitignore
## Archivos de Visual Studio y .NET
*.swp
*.*~
project.lock.json
.DS_Store
*.pyc
nupkg/

# User-specific files
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
build/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio cache/options directory
.vs/

# Data files (runtime generated)
data/*.json
data/logs/

# Network configuration backups
config/**/*.backup
config/**/*.bak

# SSL Certificates
config/certificates/*.pem
config/certificates/*.key
config/certificates/*.crt

# Temporary files
*.tmp
*.temp
```

---

Esta estructura proporciona:

✅ **Separación clara de responsabilidades** por proyecto  
✅ **Fácil navegación** y mantenimiento  
✅ **Testabilidad** con proyectos de pruebas separados  
✅ **Escalabilidad** para agregar nuevas características  
✅ **Organización profesional** siguiendo mejores prácticas de .NET

¿Quieres que comencemos a implementar algún proyecto específico?