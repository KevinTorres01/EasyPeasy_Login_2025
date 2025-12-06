#!/bin/bash

set -e

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
WEB_PROJECT="$PROJECT_DIR/src/EasyPeasy_Login.Web"

echo "🚀 EasyPeasy Login - Starting Application"
echo "=========================================="
echo ""

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}📍 Project Directory: $PROJECT_DIR${NC}"
echo ""

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${YELLOW}❌ .NET SDK is not installed!${NC}"
    echo "Please install .NET 10.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

echo -e "${GREEN}✓ .NET SDK found${NC}"
DOTNET_VERSION=$(dotnet --version)
echo "  Version: $DOTNET_VERSION"
echo ""

# Clean previous builds
echo -e "${BLUE}🧹 Cleaning previous builds...${NC}"
if [ -d "$PROJECT_DIR/bin" ]; then
    rm -rf "$PROJECT_DIR/bin"
fi
if [ -d "$PROJECT_DIR/src/*/bin" ]; then
    find "$PROJECT_DIR/src" -type d -name "bin" -exec rm -rf {} + 2>/dev/null || true
fi
if [ -d "$PROJECT_DIR/src/*/obj" ]; then
    find "$PROJECT_DIR/src" -type d -name "obj" -exec rm -rf {} + 2>/dev/null || true
fi
echo -e "${GREEN}✓ Clean complete${NC}"
echo ""

# Build the solution
echo -e "${BLUE}🔨 Building solution...${NC}"
cd "$PROJECT_DIR"
dotnet build -c Debug

if [ $? -ne 0 ]; then
    echo -e "${YELLOW}❌ Build failed!${NC}"
    exit 1
fi

echo -e "${GREEN}✓ Build successful${NC}"
echo ""

# Display application info
echo -e "${BLUE}📋 Application Ports & Access:${NC}"
echo "  ├─ HttpServer (Captive Portal):  http://192.168.100.1:8080"
echo "  │  └─ Login Page:                http://192.168.100.1:8080/portal/login"
echo "  │  └─ Success Page:              http://192.168.100.1:8080/portal/success"
echo "  │  └─ Default Admin User:        admin / admin05"
echo "  │"
echo "  └─ ASP.NET Admin Panel:         http://192.168.100.1:5000"
echo "     └─ Admin Dashboard:           http://192.168.100.1:5000/admin"
echo "     └─ Blazor UI on port 5000"
echo ""

# Start the application
echo -e "${BLUE}▶️  Starting application...${NC}"
echo -e "${YELLOW}Note: The application will run in the foreground.${NC}"
echo -e "${YELLOW}Press Ctrl+C to stop.${NC}"
echo ""

cd "$WEB_PROJECT"
dotnet run -c Debug

