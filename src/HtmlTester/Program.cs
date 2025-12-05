using System;
using System.IO;
using EasyPeasy_Login.Server.HtmlPages;
using EasyPeasy_Login.Server.HtmlPages.Admin;

Console.WriteLine("╔══════════════════════════════════════════════════════╗");
Console.WriteLine("║   GENERADOR DE PÁGINAS HTML - PORTAL CAUTIVO        ║");
Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

// Crear carpeta de salida
string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "generated_html");
if (!Directory.Exists(outputDir))
{
    Directory.CreateDirectory(outputDir);
}

Console.WriteLine($"📁 Carpeta de salida: {outputDir}\n");

// 1. Generar LoginPage
Console.WriteLine("🔐 Generando LoginPage...");
GenerateLoginPages(outputDir);

// 2. Generar SuccessPage
Console.WriteLine("\n🎉 Generando SuccessPage...");
GenerateSuccessPages(outputDir);

// 3. Generar TermsPage
Console.WriteLine("\n📜 Generando TermsPage...");
GenerateTermsPages(outputDir);

// 4. Generar Admin Pages
Console.WriteLine("\n👨‍💼 Generando Admin Pages...");
GenerateAdminPages(outputDir);

// Resumen
Console.WriteLine("\n╔══════════════════════════════════════════════════════╗");
Console.WriteLine("║   ✅ GENERACIÓN COMPLETADA                          ║");
Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

Console.WriteLine("📂 Archivos generados:");
var files = Directory.GetFiles(outputDir, "*.html");
foreach (var file in files)
{
    var fileInfo = new FileInfo(file);
    Console.WriteLine($"   ✓ {Path.GetFileName(file)} ({fileInfo.Length:N0} bytes)");
}

Console.WriteLine($"\n🌐 Para ver las páginas, abre los archivos HTML en tu navegador:");
Console.WriteLine($"   xdg-open {outputDir}/login.html");
Console.WriteLine($"\n   O navega a: {outputDir}");

static void GenerateLoginPages(string outputDir)
{
    // Login limpio
    string loginClean = LoginPage.GenerateCleanLoginPage(
        clientIp: "192.168.1.100",
        clientMac: "AA:BB:CC:DD:EE:FF"
    );
    File.WriteAllText(Path.Combine(outputDir, "login.html"), loginClean);
    Console.WriteLine($"   ✓ login.html ({loginClean.Length:N0} bytes)");

    // Login con error
    string loginError = LoginPage.GenerateLoginPageWithError(
        errorMessage: "Invalid username or password. Please try again.",
        username: "admin",
        clientIp: "192.168.1.100",
        clientMac: "AA:BB:CC:DD:EE:FF"
    );
    File.WriteAllText(Path.Combine(outputDir, "login_with_error.html"), loginError);
    Console.WriteLine($"   ✓ login_with_error.html ({loginError.Length:N0} bytes)");

    // Test de parsing
    string testPostData = "username=admin&password=admin123&acceptTerms=on";
    var formData = LoginPage.ParseLoginFormData(testPostData);
    Console.WriteLine($"   ℹ️  Test parsing: User={formData?.Username}, Terms={formData?.AcceptTerms}");
}

static void GenerateSuccessPages(string outputDir)
{
    // Success principal (inglés por defecto)
    string success = SuccessPage.GenerateSuccessPageEnglish();
    File.WriteAllText(Path.Combine(outputDir, "success.html"), success);
    Console.WriteLine($"   ✓ success.html ({success.Length:N0} bytes)");

    // Success en español
    string successEs = SuccessPage.GenerateSuccessPageSpanish();
    File.WriteAllText(Path.Combine(outputDir, "success_es.html"), successEs);
    Console.WriteLine($"   ✓ success_es.html ({successEs.Length:N0} bytes)");

    // Success en inglés (duplicado para consistencia)
    string successEn = SuccessPage.GenerateSuccessPageEnglish();
    File.WriteAllText(Path.Combine(outputDir, "success_en.html"), successEn);
    Console.WriteLine($"   ✓ success_en.html ({successEn.Length:N0} bytes)");

    // Success personalizado
    string successCustom = SuccessPage.GenerateCustomSuccessPage(
        title: "¡Felicidades!",
        subtitle: "Tu sesión ha sido iniciada correctamente"
    );
    File.WriteAllText(Path.Combine(outputDir, "success_custom.html"), successCustom);
    Console.WriteLine($"   ✓ success_custom.html ({successCustom.Length:N0} bytes)");
}

static void GenerateTermsPages(string outputDir)
{
    // Terms en español
    string termsEs = TermsPage.GenerateTermsPageSpanish();
    File.WriteAllText(Path.Combine(outputDir, "terms_es.html"), termsEs);
    Console.WriteLine($"   ✓ terms_es.html ({termsEs.Length:N0} bytes)");

    // Terms en inglés
    string termsEn = TermsPage.GenerateTermsPageEnglish();
    File.WriteAllText(Path.Combine(outputDir, "terms_en.html"), termsEn);
    Console.WriteLine($"   ✓ terms_en.html ({termsEn.Length:N0} bytes)");
}

static void GenerateAdminPages(string outputDir)
{
    // Dashboard
    string dashboard = DashboardPage.GenerateDashboard(
        usersConnected: 12,
        activeSessions: 8,
        registeredUsers: 45
    );
    File.WriteAllText(Path.Combine(outputDir, "admin_dashboard.html"), dashboard);
    Console.WriteLine($"   ✓ admin_dashboard.html ({dashboard.Length:N0} bytes)");
    
    // Network Control - Network Inactive
    string networkInactive = NetworkControlPage.GenerateNetworkControl(
        isNetworkActive: false,
        wifiInterface: "wlan0",
        ssid: "EasyPeasy WiFi",
        password: "easypeasy123",
        gatewayIp: "192.168.4.1",
        defaultPort: 80,
        dhcpRange: "192.168.4.50,192.168.4.150",
        consoleExpanded: false,
        sampleLogs: null
    );
    File.WriteAllText(Path.Combine(outputDir, "admin_network_inactive.html"), networkInactive);
    Console.WriteLine($"   ✓ admin_network_inactive.html ({networkInactive.Length:N0} bytes)");
    
    // Network Control - Network Active
    string networkActive = NetworkControlPage.GenerateNetworkControl(
        isNetworkActive: true,
        upstreamInterface: "eth0",
        isVpnInterface: false,
        wifiInterface: "wlan0",
        ssid: "EasyPeasy WiFi",
        password: "easypeasy123",
        gatewayIp: "192.168.4.1",
        defaultPort: 80,
        dhcpRange: "192.168.4.50,192.168.4.150",
        consoleExpanded: true,
        sampleLogs: NetworkControlPage.GenerateNetworkStartupLogs()
    );
    File.WriteAllText(Path.Combine(outputDir, "admin_network_active.html"), networkActive);
    Console.WriteLine($"   ✓ admin_network_active.html ({networkActive.Length:N0} bytes)");
    
    // User Management
    var sampleUsers = new List<(string name, string username, bool isActive)>
    {
        ("John Doe", "admin", true),
        ("Jane Smith", "user1", true),
        ("Mike Johnson", "mike.j", false),
        ("Sarah Williams", "sarah.w", true),
        ("Robert Brown", "robert.b", true),
        ("Emma Davis", "emma.d", true),
        ("David Wilson", "david.w", false),
        ("Lisa Anderson", "lisa.a", true)
    };
    
    string userManagement = UserManagementPage.GenerateUserManagement(sampleUsers);
    File.WriteAllText(Path.Combine(outputDir, "admin_users.html"), userManagement);
    Console.WriteLine($"   ✓ admin_users.html ({userManagement.Length:N0} bytes)");
    
    // Connected Devices
    var sampleDevices = new List<(string ipAddress, string macAddress, string username)>
    {
        ("192.168.4.10", "AA:BB:CC:DD:EE:01", "admin"),
        ("192.168.4.15", "AA:BB:CC:DD:EE:02", "user1"),
        ("192.168.4.20", "AA:BB:CC:DD:EE:03", "sarah.w"),
        ("192.168.4.25", "AA:BB:CC:DD:EE:04", "mike.j"),
        ("192.168.4.30", "AA:BB:CC:DD:EE:05", "robert.b"),
        ("192.168.4.35", "AA:BB:CC:DD:EE:06", "emma.d"),
        ("192.168.4.40", "AA:BB:CC:DD:EE:07", "lisa.a")
    };
    
    string devices = DevicesPage.GenerateDevices(sampleDevices);
    File.WriteAllText(Path.Combine(outputDir, "admin_devices.html"), devices);
    Console.WriteLine($"   ✓ admin_devices.html ({devices.Length:N0} bytes)");
}
