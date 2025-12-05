#!/usr/bin/env bash
# Script para ejecutar tareas del proyecto con sudo preservando variables de entorno críticas cuando sea posible.
# Uso:
#   ./scripts/run_with_sudo.sh run|watch|build|publish [-- extra dotnet args]

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
DEFAULT_PROJECT="$REPO_ROOT/src/EasyPeasy_Login.Web/EasyPeasy_Login.Web.csproj"
SOLUTION_FILE="$REPO_ROOT/EasyPeasy_Login.sln"

print_help() {
  cat <<EOF
Uso: $0 [run|watch|build|publish] [-- additional dotnet args]

Acciones:
  run     - Ejecuta la aplicación con 'dotnet run' en el proyecto web
  watch   - Ejecuta 'dotnet watch run' en el proyecto web
  build   - Ejecuta 'dotnet build' sobre la solución
  publish - Ejecuta 'dotnet publish' sobre la solución

Ejemplo:
  sudo ./scripts/run_with_sudo.sh run
  ./scripts/run_with_sudo.sh watch -- --urls "http://*:5000"
EOF
}

if ! command -v dotnet >/dev/null 2>&1; then
  echo "Error: dotnet no está instalado o no está en PATH." >&2
  exit 1
fi

if [ ! -f "$DEFAULT_PROJECT" ]; then
  echo "Error: no se encontró el proyecto esperado en: $DEFAULT_PROJECT" >&2
  echo "Asegúrate de ejecutar el script desde la raíz del repo o ajusta DEFAULT_PROJECT." >&2
  exit 1
fi

# Ejecuta un comando con sudo intentando preservar variables de entorno relevantes.
run_sudo_cmd() {
  # Preferimos preservar DOTNET_ROOT y PATH para que dotnet se encuentre igual.
  if sudo --preserve-env=DOTNET_ROOT,PATH true 2>/dev/null; then
    sudo --preserve-env=DOTNET_ROOT,PATH "$@"
  elif sudo -E true 2>/dev/null; then
    sudo -E "$@"
  else
    echo "Advertencia: no fue posible preservar variables de entorno con sudo; ejecutando sudo sin preservar env." >&2
    sudo "$@"
  fi
}

ACTION=${1:-run}
shift || true
EXTRA_ARGS=("$@")

case "$ACTION" in
  run)
    echo "Ejecutando: dotnet run --project $DEFAULT_PROJECT ${EXTRA_ARGS[*]}"
    run_sudo_cmd dotnet run --project "$DEFAULT_PROJECT" "${EXTRA_ARGS[@]}"
    ;;
  watch)
    echo "Ejecutando: dotnet watch --project $DEFAULT_PROJECT run ${EXTRA_ARGS[*]}"
    run_sudo_cmd dotnet watch --project "$DEFAULT_PROJECT" run "${EXTRA_ARGS[@]}"
    ;;
  build)
    echo "Ejecutando: dotnet build $SOLUTION_FILE"
    run_sudo_cmd dotnet build "$SOLUTION_FILE" --property:GenerateFullPaths=true "${EXTRA_ARGS[@]}"
    ;;
  publish)
    echo "Ejecutando: dotnet publish $SOLUTION_FILE"
    run_sudo_cmd dotnet publish "$SOLUTION_FILE" --property:GenerateFullPaths=true "${EXTRA_ARGS[@]}"
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
