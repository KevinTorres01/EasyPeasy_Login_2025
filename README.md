# 🌐 EasyPeasy Login - Captive Portal System

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![Linux](https://img.shields.io/badge/Linux-FCC624?style=for-the-badge&logo=linux&logoColor=black)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**Complete Captive Portal System for WiFi network access management built with .NET 9 and N-Layered Architecture**

[About](#-about) • [Features](#-features) • [Architecture](#-architecture) • [Prerequisites](#-prerequisites) • [Installation](#-installation) • [Usage](#-usage)

</div>

---

## 📋 Overview

**EasyPeasy Login** is an enterprise captive portal system developed in C# with .NET 9 that enables secure and controlled WiFi network access management. The system creates a WiFi access point with an authentication portal that intercepts network traffic and requires users to authenticate before gaining Internet access.

The project implements an **N-Layered architecture** with clear separation of responsibilities between layers, facilitating maintenance, testing, and system scalability.

---

## Features

### 🔐 Security and Authentication
- **Modern UI/UX login portal** - Attractive interface with interactive SVG animations
- **Credential-based authentication** - User system with BCrypt hashed passwords
- **MAC-based access control** - Device identification and authorization by MAC address
- **Session management** - Active session control with revocation capability

### 📡 Automated Network Configuration
- **Automatic Access Point (AP) creation** - hostapd configuration for WiFi access point
- **Integrated DHCP** - dnsmasq server for automatic IP assignment
- **Controlled DNS Spoofing** - DNS traffic redirection to force authentication
- **Dynamic Firewall (iptables)** - Granular traffic control per device

### 🖥️ Administration Panel
- **Real-time dashboard** - System status visualization
- **User management** - Complete CRUD of users with activation/deactivation
- **Device management** - Visualization and disconnection of connected devices
- **Network control** - Portal start/stop with hot configuration
- **Real-time logs** - Logging system with live events

### 🔧 Technical Features
- **Custom HTTP server** - HTTP server implementation with sockets for maximum control
- **Multi-OS compatibility** - Captive portal detection for Android, iOS, Windows, macOS and Linux
- **VPN support** - Detection and special configuration for VPN interfaces
- **JSON persistence** - Thread-safe JSON file data storage
- **Modular architecture** - N-Layered architecture with clear separation of responsibilities

---

## Architecture

The project implements an **N-Layered architecture** organized in horizontal layers with descending dependencies. Unlike Clean Architecture where dependencies point towards the domain, here upper layers can depend on lower ones following a traditional layer model.

### Why N-Layered and not Clean Architecture?

Although the project shares some principles with Clean Architecture (separation of concerns, use of interfaces), there are key differences:

| Aspect | Clean Architecture | This Project (N-Layered) |
|---------|-------------------|---------------------------|
| **Dependencies** | Point towards the center (Domain) | Descending (upper layers → lower) |
| **Infrastructure** | Only implements Domain interfaces | References Application directly |
| **Dependency inversion** | Strict across all layers | Partial (some interfaces in Application) |
| **Domain isolation** | Domain without external dependencies | Domain without project dependencies ✓ |

```
┌─────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                           │
│  ┌──────────────────────┐  ┌─────────────────────────────────┐  │
│  │  EasyPeasy_Login.Web │  │  EasyPeasy_Login.Server         │  │
│  │  (Blazor Server)     │  │  (Custom HTTP Server)           │  │
│  │                      │  │                                 │  │
│  │  References:         │  │  References:                    │  │
│  │  • Application       │  │  • Application                  │  │
│  │  • Infrastructure    │  │  • Infrastructure               │  │
│  │  • Shared            │  │  • Shared                       │  │
│  └──────────────────────┘  └─────────────────────────────────┘  │
├─────────────────────────────────────────────────────────────────┤
│                    INFRASTRUCTURE LAYER                         │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │               EasyPeasy_Login.Infrastructure             │   │
│  │                                                          │   │
│  │  References:                                             │   │
│  │  • Application (for service interfaces)                  │   │
│  │  • Domain (for entities and repository interfaces)       │   │
│  │  • Shared (for constants and logger)                     │   │
│  └──────────────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────────────┤
│                    APPLICATION LAYER                            │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │                 EasyPeasy_Login.Application              │   │
│  │                                                          │   │
│  │  References:                                             │   │
│  │  • Domain (for entities and interfaces)                  │   │
│  │  • Shared (for logger and constants)                     │   │
│  └──────────────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────────────┤
│                 DOMAIN & SHARED LAYER (Base)                    │
│  ┌─────────────────────┐  ┌─────────────────────────────────┐   │
│  │ EasyPeasy_Login.    │  │  EasyPeasy_Login.Shared         │   │
│  │ Domain              │  │                                 │   │
│  │                     │  │  No references to other         │   │
│  │ No references to    │  │  system projects                │   │
│  │ other projects      │  │                                 │   │
│  └─────────────────────┘  └─────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

### Project Dependencies Diagram

```
                    ┌─────────────────┐
                    │  EasyPeasy_     │
                    │  Login.Web      │
                    └────────┬────────┘
                             │
              ┌──────────────┼──────────────┐
              │              │              │
              ▼              ▼              ▼
    ┌─────────────────┐ ┌─────────────────┐ ┌─────────┐
    │ Infrastructure  │ │   Application   │ │ Shared  │
    │                 │ │                 │ │         │
    └────────┬────────┘ └────────┬────────┘ └─────────┘
             │                   │
    ┌────────┴────────┐          │
    │                 │          │
    ▼                 ▼          ▼
┌─────────────┐ ┌─────────┐  ┌─────────┐
│ Application │ │ Domain  │  │ Domain  │
│             │ │         │  │         │
└──────┬──────┘ └─────────┘  └────┬────┘
       │                          │
       ▼                          ▼
   ┌─────────┐              ┌─────────┐
   │ Domain  │              │ Shared  │
   │         │              │         │
   └────┬────┘              └─────────┘
        │
        ▼
   ┌─────────┐
   │ Shared  │
   │         │
   └─────────┘


    ┌─────────────────┐
    │ EasyPeasy_      │  (Independent project - can run separately)
    │ Login.Server    │
    └────────┬────────┘
             │
    ┌────────┴───────┬──────────────────┐
    │                │                  │
    ▼                ▼                  ▼
┌──────────────┐ ┌──────────────┐ ┌─────────┐
│Infrastructure│ │ Application  │ │ Shared  │
│              │ │              │ │         │
└──────┬───────┘ └──────┬───────┘ └─────────┘
       │                │
┌──────┴───────┐        │
│              │        │
▼              ▼        ▼
┌───────────┐ ┌────────┐ ┌─────────┐
│Application│ │ Domain │ │ Domain  │
│           │ │        │ │         │
└────┬──────┘ └────────┘ └────┬────┘
     │                        │
     ▼                        ▼
┌─────────┐              ┌─────────┐
│ Domain  │              │ Shared  │
│         │              │         │
└────┬────┘              └─────────┘
     │
     ▼
┌─────────┐
│ Shared  │
│         │
└─────────┘
```

### Projects and References

#### 1. **EasyPeasy_Login.Domain** (Core - No external dependencies)
The innermost and most stable layer of the system. Has no dependencies on other project layers.

```
EasyPeasy_Login.Domain/
├── Entities/           # Domain entities (User, Device, Session)
├── Interfaces/         # Repository contracts (IRepository, IUserRepository...)
├── Exceptions/         # Custom domain exceptions
└── Helper/             # Domain utilities (IPasswordHasher)
```

**Dependencies:** `BCrypt.Net-Next` (password hashing)

#### 2. **EasyPeasy_Login.Shared** (Cross-cutting concerns)
Contains shared utilities and constants used by all layers.

```
EasyPeasy_Login.Shared/
├── Constants/          # NetworkConstants, PersistenceConstants
└── Logger/             # ILogger, Logger (custom logging system)
```

**Dependencies:** None

#### 3. **EasyPeasy_Login.Application** (Use cases)
Implements business logic and application use cases.

```
EasyPeasy_Login.Application/
├── DTOs/               # Data Transfer Objects (Request/Response)
└── Services/
    ├── Authentication/        # Authentication service
    ├── UserManagement/        # User management
    ├── SessionManagement.cs/  # Session management
    ├── DeviceManagement.cs/   # Device management
    └── NetworkControl/        # Network access control (Firewall)
```

**References:** `Domain`, `Shared`

#### 4. **EasyPeasy_Login.Infrastructure** (Implementations)
Implements interfaces defined in Domain and provides access to external resources.

```
EasyPeasy_Login.Infrastructure/
├── Persistance/
│   └── Repositories/   # JSON repository implementations
└── Network/
    ├── Configuration/  # Network configuration (see detailed section)
    └── Helpers/        # MacAddressResolver
```

**References:** `Domain`, `Application`, `Shared`

#### 5. **EasyPeasy_Login.Server** (Custom HTTP Server)
HTTP server implemented with sockets for the captive portal.

```
EasyPeasy_Login.Server/
├── Checking/
│   ├── HttpServer.cs       # HTTP server with sockets
│   ├── HttpPetition.cs     # HTTP request parser
│   ├── ApiRouter.cs        # REST API router
│   └── ApiResponseBuilder.cs # HTTP response builder
└── HtmlPages/
    ├── LoginPage.cs        # Login page (generated HTML)
    ├── SuccessPage.cs      # Success page
    └── Admin/              # Administration pages
```

**References:** `Application`, `Infrastructure`, `Shared`

#### 6. **EasyPeasy_Login.Web** (Blazor Server)
Blazor Server web application with administration interface.

```
EasyPeasy_Login.Web/
├── Components/
│   ├── Pages/
│   │   ├── Admin/      # Dashboard, UserManagement, Devices, NetworkControl
│   │   └── Portal/     # Login, Success, Terms
│   ├── Portal/         # Animated avatar components
│   └── Shared/         # Shared components
├── Controllers/
│   └── Api/            # NetworkController (REST API)
├── Middleware/         # RequestRouterMiddleware (request interceptor)
└── HostedServices/     # Background services
```

**References:** `Application`, `Infrastructure`, `Shared`

---

## Infrastructure/Network/Configuration

This is one of the most sophisticated parts of the project. It implements the **complete network configuration** necessary to operate a functional captive portal on Linux.

### Structure

```
Network/Configuration/
├── INetworkOrchestrator.cs          # Orchestrator interface
├── NetworkOrchestrator.cs           # Complete configuration orchestration
├── ExecutionResult.cs               # Record for command results
├── CommandExecutor/
│   ├── ICommandExecutor.cs          # Interface for command execution
│   └── CommandExecutor.cs           # Bash command executor
├── NetworkConfiguration/
│   ├── INetworkConfiguration.cs     # Configuration interface
│   └── NetworkConfiguration.cs      # Network configuration state
├── NetworkManager/
│   ├── INetworkManager.cs           # Interface for network interface management
│   ├── NetworkManager.cs            # Implementation (ip link, nmcli, sysctl)
│   └── NetworkConfigurationCommands.cs # Static commands
├── Hostapd/
│   ├── IHostapdManager.cs           # Interface for Access Point
│   ├── HostapdManager.cs            # hostapd configuration and control
│   └── HostapdCommands.cs           # hostapd commands
├── Dnsmasq/
│   ├── IDnsmasqManager.cs           # Interface for DHCP/DNS
│   ├── DnsmasqManager.cs            # dnsmasq configuration
│   └── DnsmasqCommands.cs           # dnsmasq commands
└── Iptables/
    ├── ICaptivePortalControlManager.cs  # Interface for portal firewall
    ├── CaptivePortalControlManager.cs   # iptables configuration
    ├── IptablesCommands.cs              # iptables commands (60+ commands)
    └── FirewallService.cs               # Service to grant/revoke access
```

### Design Pattern: Facade + Strategy + Command

Network configuration implements multiple design patterns:

#### **Facade Pattern** - `NetworkOrchestrator`
The orchestrator acts as a facade that simplifies the complexity of configuring multiple subsystems:

```csharp
public class NetworkOrchestrator : INetworkOrchestrator
{
    // Coordinates all managers
    public async Task<bool> SetUpNetwork()
    {
        // 1. Detect upstream interface
        // 2. Unblock RF-kill
        // 3. Configure network interface
        // 4. Start hostapd (Access Point)
        // 5. Enable IP forwarding
        // 6. Start dnsmasq (DHCP/DNS)
        // 7. Configure iptables (Captive Portal)
    }
}
```

#### **Strategy Pattern** - Interchangeable managers
Each manager implements an interface that allows swapping implementations:
- `IDnsmasqManager` → `DnsmasqManager`
- `IHostapdManager` → `HostapdManager`
- `INetworkManager` → `NetworkManager`
- `ICaptivePortalControlManager` → `CaptivePortalControlManager`

#### ⚡ **Command Pattern** - Static command classes
System commands are encapsulated in static classes with descriptive methods:

```csharp
public static class IptablesCommands
{
    public static string GrantInternetAccessToMac(string macAddress)
        => $"iptables -I AUTHENTICATED 1 -m mac --mac-source {macAddress} -j ACCEPT";
    
    public static string RedirectHttpTrafficToPortal(string iface, string portalIp, int port)
        => $"iptables -t nat -A PREROUTING -i {iface} -p tcp --dport 80 -j DNAT --to-destination {portalIp}:{port}";
}
```

### Network Configuration Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    NetworkOrchestrator.SetUpNetwork()           │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│  1. DetectUpstreamInterface()                                   │
│     Search for interface with Internet access (eth0, usb0...)   │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│  2. UnblockRfkill() + ConfigureNetworkInterface()               │
│     - rfkill unblock wifi                                       │
│     - nmcli device set wlp2s0 managed no                        │
│     - ip addr add 192.168.100.1/24 dev wlp2s0                   │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│  3. HostapdManager.ConfigureHostapdAsync()                      │
│     Creates /etc/hostapd/hostapd.conf with SSID and password    │
│     Starts hostapd -B (background mode)                         │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│  4. DnsmasqManager.ConfigureDnsmasqAsync()                      │
│     - Stops systemd-resolved                                    │
│     - Configures DHCP range: 192.168.100.50-150                 │
│     - DNS Spoofing: All queries → Gateway IP                    │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│  5. CaptivePortalControlManager.ConfigureCaptivePortal()        │
│     - Clears existing rules                                     │
│     - Redirects DNS (port 53) to gateway                        │
│     - Default policies: INPUT=ACCEPT, FORWARD=DROP              │
│     - Allows DHCP, DNS, ICMP, port 8080                         │
│     - Redirects HTTP/HTTPS to portal                            │
│     - Creates AUTHENTICATED chain for authenticated users       │
│     - Configures NAT/MASQUERADE to upstream interface           │
└─────────────────────────────────────────────────────────────────┘
```

### Dynamic Access Management (FirewallService)

When a user successfully authenticates:

```csharp
public async Task<bool> GrantInternetAccessAsync(string macAddress)
{
    // 1. Add FORWARD rule in AUTHENTICATED chain
    await _executor.ExecuteCommandAsync(
        IptablesCommands.GrantInternetAccessToMac(macAddress));
    
    // 2. Bypass HTTP/HTTPS redirection for this MAC
    await _executor.ExecuteCommandAsync(
        IptablesCommands.BypassHttpRedirectForMac(iface, macAddress));
    
    // 3. Redirect DNS to external server (8.8.8.8)
    await _executor.ExecuteCommandAsync(
        IptablesCommands.RedirectDnsToExternalForMac(iface, macAddress));
}
```

---

## Custom HTTP Server

The project includes an HTTP server implemented from scratch using **TCP sockets**, without dependencies on frameworks like Kestrel or HttpListener.

### Why a custom server?

1. **Full control over the protocol** - Necessary to correctly respond to captive portal checks from different operating systems
2. **Low footprint** - Doesn't require the full ASP.NET Core stack
3. **Educational** - Demonstrates how HTTP works at the protocol level
4. **Flexibility** - Allows custom HTTP responses without framework restrictions

### Server Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         HttpServer                              │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  Socket Listener (Port 8080)                              │  │
│  │  - AcceptAsync() → HandleClientAsync()                    │  │
│  └───────────────────────────────────────────────────────────┘  │
│                              │                                  │
│                              ▼                                  │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  HttpPetition.Parse()                                     │  │
│  │  Extracts: Method, Path, Host, UserAgent, Body, ClientIP  │  │
│  └───────────────────────────────────────────────────────────┘  │
│                              │                                  │
│                              ▼                                  │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  ProcessRequestAsync() - Decision flow                    │  │
│  │  1. Is OPTIONS? → CORS Preflight                          │  │
│  │  2. Is connectivity check? → 204 or Redirect based on auth│  │
│  │  3. Is /api/*? → ApiRouter                                │  │
│  │  4. Is localhost? → Admin dashboard                       │  │
│  │  5. Is /admin/*? → Administration pages                   │  │
│  │  6. Is /portal/*? → Login/Success pages                   │  │
│  │  7. Default → Check session and respond                   │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### Captive Portal Detection

The server automatically detects connectivity checks from different operating systems:

```csharp
private bool IsConnectivityCheck(HttpPetition request)
{
    string path = request.Path.ToLower();
    string host = request.Host.ToLower();
    string ua = request.UserAgent.ToLower();

    // Known verification paths
    // - Android: /generate_204, /gen_204
    // - iOS/macOS: /hotspot-detect.html, /library/test/success.html
    // - Windows: /ncsi.txt, /connecttest.txt
    // - Firefox: /success.txt
    // - Ubuntu: /canonical.html
    
    // Known hosts
    // - apple.com, gstatic.com, msft*, firefox*, ubuntu*, android*
    
    // Specific User-Agents
    // - CaptiveNetworkSupport (iOS/macOS)
    // - Microsoft NCSI (Windows)
    // - Dalvik (Android)
}
```

### API Router

The `ApiRouter` handles all REST endpoints under `/api/*`:

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/users` | GET | List all users |
| `/api/users` | POST | Create a new user |
| `/api/users/{username}` | PUT | Update a user |
| `/api/users/{username}` | DELETE | Delete a user |
| `/api/device` | GET | List connected devices |
| `/api/device/{mac}` | DELETE | Disconnect a device |
| `/api/session` | GET | List active sessions |
| `/api/session/{mac}` | DELETE | Terminate a session |
| `/api/network/start` | POST | Start network configuration |
| `/api/network/stop` | POST | Stop and restore network |
| `/api/network/status` | GET | Current network status |

### HTML Generation

HTML pages are generated programmatically in C# classes:

```csharp
public static class LoginPage
{
    public static string GenerateCleanLoginPage(string clientIp, string clientMac)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang='en'>");
        // ... complete HTML generation with inline CSS and JavaScript
        return html.ToString();
    }
}
```

This allows:
- **Self-contained pages** without static file dependencies
- **Dynamic data injection** (client IP, MAC, error messages)
- **Interactive SVG animations** (eye tracking, mouth animations)

---

## Project Strengths

### 1. **Well-Structured N-Layered Architecture**
- Clear separation between layers (Domain, Application, Infrastructure, Presentation)
- Domain and Shared without dependencies on other projects (stable base layers)
- Maintainable code thanks to modular organization
- Easy extensibility to add new features

### 2. **Professional Network System**
- Complete WiFi Access Point configuration
- Dynamic firewall with granular MAC control
- Support for multiple upstream connection types (Ethernet, USB tethering, VPN)
- Automatic configuration restoration on stop

### 3. **Multi-Platform Compatibility**
- Captive portal detection for all major operating systems
- HTTP responses adapted to each client type
- Responsive UI for mobile devices

### 4. **Robust Security**
- BCrypt hashed passwords
- MAC address-based access control
- Network isolation until successful authentication
- Admin access restricted to localhost

### 5. **Two Operation Modes**
- **Blazor Server** (`EasyPeasy_Login.Web`): Full interface with interactive components
- **HTTP Server** (`EasyPeasy_Login.Server`): Lightweight server for resource-limited environments

### 6. **Advanced Logging System**
- Real-time logs with events
- Log levels (Info, Warning, Error)
- Automatic retention (last 500 logs)
- Dashboard visualization

### 7. **Efficient Persistence**
- Thread-safe JSON repositories
- Repository pattern for persistence abstraction
- Easy migration to other technologies (SQL, NoSQL)

### 8. **Documented and Descriptive Code**
- Self-explanatory method names
- System commands encapsulated with descriptive names
- XML comments on public interfaces

### 9. **Centralized Configuration**
- Network constants in `NetworkConstants`
- Persistence constants in `PersistenceConstants`
- Runtime modifiable configuration via `INetworkConfiguration`

### 10. **Applied Design Patterns**
- **Repository Pattern** - Persistence abstraction
- **Facade Pattern** - Network subsystem orchestration
- **Strategy Pattern** - Interchangeable managers
- **Command Pattern** - System command encapsulation
- **Dependency Injection** - Dependency injection in presentation layers

---

## Prerequisites

### Hardware
- Computer with WiFi adapter compatible with AP mode (hostapd)
- Internet connection (Ethernet, USB tethering, or VPN)

### Software
- **Operating System:** Linux (tested on Ubuntu 22.04+)
- **.NET SDK:** 9.0 or higher
- **Network tools:** hostapd, dnsmasq, iptables, iproute2, network-manager, rfkill
- **Privileges:** Root (sudo) for network configuration

---

## Installation

### 1. Clone the repository
```bash
git clone https://github.com/KevinTorres01/EasyPeasy_Login_2025.git
cd EasyPeasy_Login_2025
```

### 2. Install system dependencies
```bash
sudo apt update
sudo apt install -y hostapd dnsmasq iptables iproute2 network-manager rfkill
```

### 3. Restore NuGet packages
```bash
dotnet restore
```

### 4. Build the project
```bash
dotnet build
```

### 5. Configure WiFi interface
Edit `src/EasyPeasy_Login.Shared/Constants/NetworkConfigurationConstants.cs`:
```csharp
public static readonly string Interface = "wlp2s0"; // Your WiFi interface
public static readonly string Ssid = "MyGuest_Network";
public static readonly string Password = "secure_password";
```

---

## Usage

The project includes execution scripts in the `scripts/` directory that facilitate system startup.

### Run with Blazor Server (Full interface)

```bash
# Give execution permissions (only first time)
chmod +x scripts/run_blazor_server.sh

# Run the application
sudo ./scripts/run_blazor_server.sh run

# Watch mode (recompiles automatically when detecting changes)
sudo ./scripts/run_blazor_server.sh watch

# Build only
./scripts/run_blazor_server.sh build
```

**Available URLs:**
- 🛠️ Admin Dashboard: `http://localhost:8080/admin`
- 🔑 Portal Login: `http://192.168.100.1:8080/portal/login`

### Run with HTTP Server (Lightweight server)

```bash
# Give execution permissions (only first time)
chmod +x scripts/run_captive_portal.sh

# Run the custom HTTP server
sudo ./scripts/run_captive_portal.sh

# Watch mode
sudo ./scripts/run_captive_portal.sh watch

# Build only
sudo ./scripts/run_captive_portal.sh build
```

**Available URLs:**
- 🔑 Portal Login: `http://192.168.100.1:8080/portal/login`
- 🛠️ Admin Dashboard: `http://localhost:8080/admin`
- 👥 Users: `http://localhost:8080/admin/users`
- 📱 Devices: `http://localhost:8080/admin/devices`
- 🌐 Network: `http://localhost:8080/admin/network`

### Default credentials

- **User:** `admin`
- **Password:** `admin05`

> ⚠️ **Note:** Root privileges (`sudo`) are required for network configuration (hostapd, iptables, dnsmasq).

---

## API Reference

### REST API Endpoints

The server exposes the following REST endpoints:

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/users` | GET | List all users |
| `/api/users` | POST | Create a new user |
| `/api/users/{username}` | PUT | Update a user |
| `/api/users/{username}` | DELETE | Delete a user |
| `/api/device` | GET | List connected devices |
| `/api/device/{mac}` | DELETE | Disconnect a device |
| `/api/session` | GET | List active sessions |
| `/api/session/{mac}` | DELETE | Terminate a session |
| `/api/network/start` | POST | Start network configuration |
| `/api/network/stop` | POST | Stop and restore network |
| `/api/network/status` | GET | Current network status |

### Data Storage

Data is stored in JSON files in the `data/` directory:

```
data/
├── Users.json      # System users
├── Sessions.json   # Active sessions
├── Devices.json    # Registered devices
└── backup/         # Configuration backups
```

---

## 🤝 Contributing

Contributions are welcome. Please:

1. Fork the repository
2. Create a branch for your feature (`git checkout -b feature/NewFeature`)
3. Commit your changes (`git commit -m 'Add new feature'`)
4. Push to the branch (`git push origin feature/NewFeature`)
5. Open a Pull Request

---

## 📄 License

This project is under the MIT License. See the `LICENSE` file for more details.

---

## 👥 Authors

### Kevin Torres - [@KevinTorres01](https://github.com/KevinTorres01)

**Backend & Core Development**

- Design and implementation of the **domain model** (User, Device, Session entities)
- Development of **repository interfaces** and their implementation with JSON persistence
- Complete implementation of **application layer services**:
  - `AuthenticationService` - Authentication logic and credential validation
  - `UserManagementService` - User CRUD with business validations
  - `SessionManagementService` - Session lifecycle management
  - `DeviceManagement` - Connected device control and tracking
- Development of the **custom HTTP server** (`EasyPeasy_Login.Server`):
  - HTTP parser implementation (`HttpPetition`)
  - REST API router (`ApiRouter`)
  - HTTP response builder (`ApiResponseBuilder`)
  - Multi-OS captive portal detection logic

### Lianny Revé - [@Rlianny](https://github.com/Rlianny)

**Infrastructure & Frontend Development**

- Architecture and complete implementation of the **network infrastructure layer**:
  - `NetworkOrchestrator` - Network configuration orchestration
  - `HostapdManager` - WiFi Access Point configuration and control
  - `DnsmasqManager` - DHCP and DNS server with controlled spoofing
  - `CaptivePortalControlManager` - iptables firewall configuration
  - `FirewallService` - Dynamic MAC-based access management
  - `NetworkManager` - Network interface control
  - Over **60 system commands** encapsulated in descriptive classes
- Development of the **Blazor Server project** (`EasyPeasy_Login.Web`):
  - Administration page components (Dashboard, Users, Devices, Network)
  - Portal login components with interactive SVG animations
  - Request routing middleware
- **Complete UI/UX design**:
  - Login interface with animated avatars (eye tracking, mouth animations)
  - Responsive administration dashboard
  - Programmatically generated HTML pages for the custom server

---

## Acknowledgments

- .NET community for excellent documentation
- Developers of hostapd, dnsmasq and iptables
- All project contributors

---

<div align="center">

**⭐ If this project has been useful to you, consider giving it a star ⭐**

</div>
