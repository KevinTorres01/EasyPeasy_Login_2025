# Descripción Completa del Proyecto: Portal Cautivo

## **Propósito General**
Desarrollar un sistema de control de acceso a Internet que funcione como portal cautivo, donde cualquier dispositivo que se conecte a la red local sea redirigido automáticamente a una página de autenticación y solo obtenga acceso a Internet después de validar sus credenciales correctamente.

## **Funcionalidades Principales**

### **Control de Red y Acceso**
- **Interceptación de tráfico**: Todo el tráfico HTTP/HTTPS de dispositivos no autenticados es capturado y redirigido al portal de login
- **Bloqueo selectivo**: Los dispositivos sin autenticar no pueden acceder a Internet, solo al portal cautivo
- **Desbloqueo dinámico**: Una vez autenticados, los dispositivos específicos obtienen acceso completo a Internet
- **Gateway obligatorio**: El sistema actúa como única puerta de salida a Internet para la red local

### **Autenticación y Gestión de Usuarios**
- **Sistema de login**: Página web con formulario de usuario y contraseña
- **Base de datos de usuarios**: Almacenamiento y gestión de credenciales válidas (archivo JSON)
- **Panel de administración**: Interfaz web accesible solo localmente para gestionar usuarios
- **Sesiones de acceso**: Control del tiempo de acceso y estado de conexión de cada dispositivo

### **Experiencia de Usuario**
- **Detección automática**: Navegadores y sistemas operativos detectan automáticamente la necesidad de autenticación
- **Interfaz responsive**: Página web adaptativa que funciona en móviles y desktop
- **Elementos interactivos**: Avatares animados que reaccionan a las acciones del usuario
- **Redirección inteligente**: Usuarios son llevados automáticamente a su destino original después del login

## **Componentes del Sistema**

### **Infraestructura de Red**
- **Servidor DHCP**: Asigna direcciones IP y configura el gateway por defecto
- **Servidor DNS**: Resuelve todas las consultas de dominio hacia el portal cautivo
- **Firewall (iptables)**: Implementa reglas de bloqueo y redirección de tráfico
- **NAT (Network Address Translation)**: Permite compartir una conexión a Internet entre múltiples dispositivos

### **Servicios Web**
- **Servidor HTTP/HTTPS**: Sirve las páginas web del portal y panel de administración
- **Endpoints de detección**: URLs específicas que los dispositivos usan para detectar portales cautivos
- **API de autenticación**: Procesa las credenciales y gestiona sesiones
- **Servicio de administración**: Gestiona usuarios y monitorea el estado del sistema

## **Flujos de Operación**

### **Para Usuarios Normales (Dispositivos Cliente)**
1. Se conectan a la red WiFi/hotspot
2. Reciben configuración de red automáticamente
3. Al intentar navegar, son redirigidos a la página de login
4. Introducen usuario y contraseña
5. Si son válidos, obtienen acceso completo a Internet
6. Si son inválidos, permanecen en la página de login

### **Para el Administrador (Acceso Local)**
1. Accede a una URL especial desde el equipo servidor
2. Visualiza el panel de administración
3. Gestiona usuarios (crear, eliminar, modificar)
4. Monitorea dispositivos conectados y autenticados
5. Configura opciones del sistema

## **Características de Seguridad**

### **Prevención de Evasión**
- **Aislamiento de clientes**: Los dispositivos no pueden comunicarse entre sí
- **Validación IP-MAC**: Prevención de suplantación de direcciones
- **Bloqueo DNS**: Impide el uso de servidores DNS externos
- **Interceptación HTTPS**: Redirección de tráfico seguro hacia el portal

### **Protección del Sistema**
- **Acceso administrativo restringido**: Solo desde localhost o IPs específicas
- **Restauración automática**: Recuperación de la configuración de red al cerrar la aplicación
- **Manejo seguro de credenciales**: Almacenamiento protegido de usuarios y contraseñas

## **Requisitos Técnicos**

### **Entorno de Ejecución**
- **Sistema operativo**: Linux (con capacidades de red avanzadas)
- **Lenguaje**: C# con .NET
- **Framework web**: Blazor Server
- **Privilegios**: Ejecución con permisos de administrador para configuración de red

### **Dependencias del Sistema**
- **iptables**: Para gestión de firewall y NAT
- **dnsmasq**: Para servicios DHCP y DNS
- **hostapd** (opcional): Para crear hotspot WiFi
- **OpenSSL**: Para certificados HTTPS

## **Características Adicionales (Extras)**

### **Detección Automática Nativa**
- Compatibilidad con iOS, Android, Windows, macOS
- Notificaciones del sistema que alertan sobre la necesidad de autenticación
- Apertura automática del navegador al detectar el portal cautivo

### **Seguridad Mejorada**
- HTTPS con certificados válidos (Let's Encrypt)
- Prevención de ataques de suplantación de IP
- Enmascaramiento de IP para toda la red

### **Experiencia de Usuario Avanzada**
- Interfaz con avatares animados e interactivos
- Diseño responsive y accesible
- Feedback visual inmediato a las acciones del usuario
- Página de términos y condiciones con elementos de humor

## **Escenarios de Uso**

### **Entorno Educativo/Demostración**
- Proyecto académico para la asignatura de Redes de Computadoras
- Demostración de conceptos de redes, firewall y control de acceso
- Aprendizaje de programación de sistemas y redes

### **Entorno Corporativo Básico**
- Control de acceso a Internet en redes pequeñas
- Autenticación simple para empleados o invitados
- Registro de dispositivos conectados a la red

## **Limitaciones y Consideraciones**

### **Limitaciones Técnicas**
- Requiere control completo sobre la infraestructura de red
- Depende de características específicas de Linux
- Rendimiento puede variar con muchos usuarios concurrentes
- Configuración compleja de red inicial

### **Consideraciones de Seguridad**
- No es un sistema de seguridad empresarial completo
- Las credenciales viajan por la red (mitigado con HTTPS)
- Posibles vectores de evasión para usuarios avanzados

Este proyecto representa una implementación completa de un portal cautivo funcional, desde la infraestructura de red hasta la interfaz de usuario, con características avanzadas de seguridad y experiencia de usuario.