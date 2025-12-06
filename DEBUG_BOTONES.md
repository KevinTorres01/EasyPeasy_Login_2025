# 🔍 Guía de Depuración - Botones no Funcionan

## Problema
Los botones de Edit/Delete/Toggle en las páginas de administración no están funcionando.

## ✅ Cambios Ya Implementados

1. **CORS Headers agregados** - Las respuestas API ahora incluyen headers CORS
2. **Data Attributes implementados** - Los botones usan `data-username` y `this.dataset.username`
3. **Endpoints API verificados** - `/api/users`, `/api/device`, `/api/session` están implementados
4. **Código JavaScript correcto** - Las funciones están definidas correctamente

## 🔧 Pasos para Depurar

### 1. Verificar que el Servidor Esté Corriendo
```bash
cd "/home/kevin/Documentos/Cloned Projects/EasyPeasy_Login_2025"
sudo ./scripts/run_captive_portal.sh run
```

### 2. Abrir la Página de Administración
Abre en tu navegador:
```
http://localhost:8080/admin/users
```

### 3. Abrir la Consola del Navegador
- **Firefox**: Presiona `F12` o `Ctrl+Shift+I`
- **Chrome**: Presiona `F12` o `Ctrl+Shift+J`

### 4. Buscar Errores en la Consola

#### ❌ Error Posible 1: "ReferenceError: openEditUserModal is not defined"
**Causa**: Las funciones JavaScript no se están cargando
**Solución**: Verificar que el `<script>` tag esté presente en el HTML

#### ❌ Error Posible 2: "CORS policy: No 'Access-Control-Allow-Origin' header"
**Causa**: CORS no está funcionando (aunque ya lo agregamos)
**Solución**: Ya implementado - verificar que el servidor tenga los cambios

#### ❌ Error Posible 3: "Failed to fetch" o "NetworkError"
**Causa**: El servidor no está respondiendo o la URL es incorrecta
**Solución**: Verificar que http://localhost:8080/api/users responde

### 5. Prueba Manual en la Consola

En la consola del navegador, escribe:
```javascript
// Test 1: Verificar que las funciones existen
typeof openEditUserModal
typeof openDeleteModal
typeof toggleUserStatus

// Test 2: Llamar directamente
openEditUserModal('test_user')

// Test 3: Verificar data attributes
document.querySelectorAll('[data-username]').length

// Test 4: Ver el primer botón
document.querySelector('[data-username]')

// Test 5: Simular clic
document.querySelector('[data-username]').click()
```

### 6. Verificar HTML Generado

En la consola del navegador:
```javascript
// Ver el HTML de la tabla
document.getElementById('usersTableBody').innerHTML
```

Deberías ver algo como:
```html
<button data-username="admin" onclick="openEditUserModal(this.dataset.username)">
```

### 7. Test de Red (Network Tab)

1. Abre la pestaña "Network" en las DevTools
2. Recarga la página
3. Verifica que `/admin/users` retorne HTML
4. Clic en un botón Edit
5. Deberías ver una llamada a `/api/users/username`

## 🎯 Prueba Rápida

Abre: `file:///home/kevin/Documentos/Cloned%20Projects/EasyPeasy_Login_2025/test_buttons_complete.html`

Si los botones funcionan en esta página, entonces el código JavaScript es correcto.

## 📋 Checklist

- [ ] Servidor corriendo en puerto 8080
- [ ] Página carga en http://localhost:8080/admin/users
- [ ] Consola del navegador abierta (F12)
- [ ] No hay errores rojos en consola
- [ ] `typeof openEditUserModal` retorna "function"
- [ ] Botones tienen atributo `data-username`
- [ ] Click en botón ejecuta la función

## 🐛 Si Aún No Funciona

**Envíame un screenshot o copia el texto de:**
1. La consola del navegador (errores en rojo)
2. El resultado de: `document.querySelector('[data-username]').outerHTML`
3. El resultado de: `typeof openEditUserModal`

Esto me permitirá identificar el problema exacto.
