# EasyPeasy Login 2025

Captive Portal System for Network Access Control with Dual Port Architecture (HttpServer + ASP.NET Core)

## 📋 Quick Start

### Option 1: Using Makefile (Recommended for Linux/macOS)
```bash
make help      # See all available commands
make run       # Build and run the application
make clean     # Clean build artifacts
```

### Option 2: Using Shell Scripts
```bash
# Linux/macOS
chmod +x run.sh
./run.sh

# Windows
run.bat
```

### Option 3: Direct dotnet commands
```bash
dotnet build EasyPeasy_Login.sln
dotnet run --project src/EasyPeasy_Login.Web/EasyPeasy_Login.Web.csproj
```

## 🏗️ Architecture

### Dual-Port Design
- **Port 8080**: HttpServer (Captive Portal)
  - Login page detection
  - Connectivity check handling
  - Session management
  
- **Port 5000**: ASP.NET Core API
  - RESTful API endpoints
  - User management API
  - Device management API
  - Session administration API

### Key Components
```
src/
├── EasyPeasy_Login.Domain/        # Core domain entities
├── EasyPeasy_Login.Application/   # Business logic (DTOs, Services)
├── EasyPeasy_Login.Infrastructure/ # Data persistence (Repositories)
├── EasyPeasy_Login.Server/        # HttpServer (Captive Portal)
└── EasyPeasy_Login.Web/           # ASP.NET Core (Admin Panel)
```

## 📖 Documentation

- **[QUICKSTART.md](./QUICKSTART.md)** - Detailed setup and usage guide
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - System architecture and data flow
- **[docs/](./docs/)** - Additional documentation

## 🔧 Prerequisites

- .NET 10.0 SDK
- Linux/macOS/Windows with bash or batch shell

## 🚀 Running the Application

### Development Mode
```bash
make run          # Builds and runs with debug symbols
```

### Watch Mode (Auto-rebuild)
```bash
make watch        # Continues running and rebuilds on file changes
```

### Production Build
```bash
make publish      # Creates optimized release build in ./publish
```

## 📱 Access Points

| Service | URL | Purpose |
|---------|-----|---------|
| **Captive Portal** | `http://192.168.100.1:8080/portal/login` | User authentication |
| **API Endpoints** | `http://192.168.100.1:5000/api/*` | REST APIs |

## 🔐 Default Credentials

| Field | Value |
|-------|-------|
| **Username** | `admin` |
| **Password** | `Admin@123` |

> ⚠️ Change immediately in production!

## 📊 Makefile Commands

```bash
make help          # Show all available commands
make build         # Build the solution
make run           # Build and run application
make clean         # Remove all build artifacts
make test          # Run unit tests
make publish       # Create production build
make watch         # Run in watch mode (auto-rebuild)
make format        # Format code with dotnet format
make restore       # Restore NuGet packages
make info          # Show project information
```

## 🌐 Environment Configuration

### Linux/macOS Network Setup
```bash
# Set static IP for WiFi interface
sudo ifconfig wlan0 192.168.100.1 netmask 255.255.255.0
sudo dnsmasq -C /etc/dnsmasq.conf
```

### Windows Network Setup
```cmd
REM Set static IP for WiFi adapter
netsh interface ip set address "Wi-Fi" static 192.168.100.1 255.255.255.0
```

## 🧪 Testing

```bash
# Run all tests
make test

# Run specific project tests
dotnet test src/EasyPeasy_Login.Application.Tests/
```

## 📦 Project Structure

### Domain Layer
- **Entities**: User, Device, Session
- **Enums**: OSType, DeviceStatus, SessionStatus
- **Interfaces**: Repository contracts
- **Exceptions**: Custom application exceptions

### Application Layer
- **DTOs**: Data transfer objects for API communication
- **Services**: Business logic implementation

### Infrastructure Layer
- **Repositories**: JSON-based persistence
- **Network**: Configuration and helpers

### Server Layer
- **HttpServer**: Custom captive portal server
- **HtmlPages**: Dynamic page generation

### Web Layer
- **ASP.NET Core**: REST API endpoints
- **Controllers**: API request handlers

## 🐛 Troubleshooting

### Port Already in Use
```bash
# Find process using port 8080
lsof -i :8080

# Kill the process
kill -9 <PID>
```

### .NET SDK Not Found
```bash
# Install .NET 10.0
curl https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version latest
```

### Build Errors
```bash
# Clean and rebuild
make clean build

# Restore packages
make restore
```

## 🔄 Updating

```bash
# Get latest changes
git pull origin main

# Rebuild with latest code
make clean run
```

## 📝 License

EasyPeasy Login 2025 - All Rights Reserved

## 👥 Contributing

For contributions, please follow:
1. Create a feature branch
2. Make your changes
3. Test with `make test`
4. Format code with `make format`
5. Submit pull request

## 📞 Support

For issues, questions, or suggestions, please open a GitHub issue or contact the development team.
