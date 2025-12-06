.PHONY: help build run clean test install-deps

# Colors
BLUE := \033[0;34m
GREEN := \033[0;32m
YELLOW := \033[1;33m
NC := \033[0m # No Color

help:
	@echo "$(BLUE)==== EasyPeasy Login - Makefile =====$(NC)"
	@echo ""
	@echo "$(GREEN)Available targets:$(NC)"
	@echo "  $(BLUE)make install-deps$(NC)   - Install .NET SDK (if not present)"
	@echo "  $(BLUE)make build$(NC)          - Build the solution"
	@echo "  $(BLUE)make run$(NC)            - Build and run the application"
	@echo "  $(BLUE)make clean$(NC)          - Clean build artifacts"
	@echo "  $(BLUE)make test$(NC)           - Run tests"
	@echo "  $(BLUE)make publish$(NC)        - Publish for production"
	@echo "  $(BLUE)make help$(NC)           - Show this help message"
	@echo ""
	@echo "$(YELLOW)Examples:$(NC)"
	@echo "  make run          # Start the application"
	@echo "  make clean        # Clean all build files"
	@echo "  make build test   # Build and test"
	@echo ""

install-deps:
	@echo "$(BLUE)Checking .NET SDK...$(NC)"
	@command -v dotnet >/dev/null 2>&1 || { \
		echo "$(YELLOW).NET SDK not found. Please install .NET 10.0 SDK from:$(NC)"; \
		echo "https://dotnet.microsoft.com/download"; \
		exit 1; \
	}
	@echo "$(GREEN)✓ .NET SDK installed: $$(dotnet --version)$(NC)"

build: install-deps
	@echo "$(BLUE)Building solution...$(NC)"
	dotnet build -c Debug
	@echo "$(GREEN)✓ Build complete$(NC)"

run: install-deps clean build
	@echo "$(BLUE)Starting application...$(NC)"
	@echo ""
	@echo "$(GREEN)📋 Ports & Access:$(NC)"
	@echo "  ├─ HttpServer (Port 8080):  http://192.168.100.1:8080"
	@echo "  │  └─ Login:                http://192.168.100.1:8080/portal/login"
	@echo "  │"
	@echo "  └─ Admin Panel (Port 5000): http://192.168.100.1:5000"
	@echo "     └─ Dashboard:            http://192.168.100.1:5000/admin"
	@echo ""
	@echo "$(YELLOW)Press Ctrl+C to stop$(NC)"
	@echo ""
	cd src/EasyPeasy_Login.Web && dotnet run -c Debug

clean:
	@echo "$(BLUE)Cleaning build artifacts...$(NC)"
	@find . -type d -name "bin" -o -name "obj" | xargs rm -rf
	@echo "$(GREEN)✓ Clean complete$(NC)"

test:
	@echo "$(BLUE)Running tests...$(NC)"
	dotnet test
	@echo "$(GREEN)✓ Tests complete$(NC)"

publish:
	@echo "$(BLUE)Publishing for production...$(NC)"
	dotnet publish -c Release -o ./publish
	@echo "$(GREEN)✓ Published to ./publish$(NC)"

watch:
	@echo "$(BLUE)Starting watch mode...$(NC)"
	dotnet watch --project src/EasyPeasy_Login.Web run

format:
	@echo "$(BLUE)Formatting code...$(NC)"
	dotnet format
	@echo "$(GREEN)✓ Format complete$(NC)"

restore:
	@echo "$(BLUE)Restoring NuGet packages...$(NC)"
	dotnet restore
	@echo "$(GREEN)✓ Restore complete$(NC)"

info:
	@echo "$(BLUE)Project Information:$(NC)"
	@echo "  Name:        EasyPeasy Login 2025"
	@echo "  Repository:  github.com/KevinTorres01/EasyPeasy_Login_2025"
	@echo "  Branch:      $$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo 'unknown')"
	@echo "  Commit:      $$(git rev-parse --short HEAD 2>/dev/null || echo 'unknown')"
	@echo ""
	@echo "$(GREEN).NET SDK Version:$(NC)"
	@dotnet --version
	@echo ""
	@echo "$(GREEN)Available Projects:$(NC)"
	@ls -1 src/*/bin/Debug/net*/EasyPeasy_Login.*.dll 2>/dev/null | sed 's|.*/||;s|.dll||' | sort | uniq | sed 's/^/  ✓ /'

.PHONY: all
all: help

