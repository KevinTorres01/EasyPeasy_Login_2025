#!/usr/bin/env bash
# Script para ejecutar el servidor HTTP del Portal Cautivo con sudo.
# Este servidor es el implementado manualmente (sin Blazor) en EasyPeasy_Login.Server
#
# Uso:
#   sudo ./scripts/run_captive_portal.sh
#   sudo ./scripts/run_captive_portal.sh build   # Solo compilar
#   sudo ./scripts/run_captive_portal.sh watch   # Modo watch

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SERVER_PROJECT="$REPO_ROOT/src/EasyPeasy_Login.Server/EasyPeasy_Login.Server.csproj"

print_header() {
  echo "╔════════════════════════════════════════════════════════════╗"
  echo "║       EasyPeasy Login - Captive Portal Server (HTTP)       ║"
  echo "╚════════════════════════════════════════════════════════════╝"
  echo ""
}

print_help() {
  cat <<EOF
Uso: sudo $0 [run|build|watch]

Acciones:
  run     - Ejecuta el servidor HTTP del portal cautivo (default)
  build   - Solo compila el proyecto
  watch   - Ejecuta en modo watch (recompila automáticamente)

URLs disponibles cuando el servidor está corriendo:
  🔑 Portal Login:    http://192.168.100.1:8080/portal/login
  🛠️  Admin Dashboard: http://localhost:8080/admin
  👥 Users:           http://localhost:8080/admin/users
  📱 Devices:         http://localhost:8080/admin/devices
  🌐 Network:         http://localhost:8080/admin/network

Ejemplo:
  sudo ./scripts/run_captive_portal.sh
  sudo ./scripts/run_captive_portal.sh build
EOF
}

# Verifica que dotnet esté instalado
if ! command -v dotnet >/dev/null 2>&1; then
  echo "Error: dotnet no está instalado o no está en PATH." >&2
  exit 1
fi

# Verifica que el proyecto exista
if [ ! -f "$SERVER_PROJECT" ]; then
  echo "Error: no se encontró el proyecto del servidor en: $SERVER_PROJECT" >&2
  exit 1
fi

# Ejecuta un comando con sudo preservando variables de entorno
run_sudo_cmd() {
  if sudo --preserve-env=DOTNET_ROOT,PATH true 2>/dev/null; then
    sudo --preserve-env=DOTNET_ROOT,PATH "$@"
  elif sudo -E true 2>/dev/null; then
    sudo -E "$@"
  else
    echo "Advertencia: ejecutando sudo sin preservar env." >&2
    sudo "$@"
  fi
}

ACTION=${1:-run}
shift || true
EXTRA_ARGS=("$@")

case "$ACTION" in
  run)
    print_header
    echo "[INFO] Iniciando servidor HTTP del Portal Cautivo..."
    echo "[INFO] Puerto: 8080"
    echo "[INFO] Gateway IP: 192.168.100.1"
    echo ""
    echo "Presiona Ctrl+C para detener el servidor."
    echo ""
    run_sudo_cmd dotnet run --project "$SERVER_PROJECT" "${EXTRA_ARGS[@]}"
    ;;
  build)
    echo "[INFO] Compilando el servidor del Portal Cautivo..."
    dotnet build "$SERVER_PROJECT" --property:GenerateFullPaths=true "${EXTRA_ARGS[@]}"
    echo "[INFO] Compilación completada."
    ;;
  watch)
    print_header
    echo "[INFO] Iniciando en modo watch..."
    run_sudo_cmd dotnet watch run --project "$SERVER_PROJECT" "${EXTRA_ARGS[@]}"
    ;;
  -h|--help|help)
    print_help
    exit 0
    ;;
  *)
    echo "Acción desconocida: $ACTION" >&2
    print_help
    exit 2
    ;;
esac
