namespace EasyPeasy_Login.Server.HtmlPages.Admin;

public static class DashboardPage
{
    private const string HTML_TEMPLATE = @"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Admin Dashboard - EasyPeasy Login</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            background-color: rgb(231, 231, 231);
            color: #000000;
            -webkit-font-smoothing: antialiased;
            -moz-osx-font-smoothing: grayscale;
        }

        .page {
            display: flex;
            min-height: 100vh;
        }

        /* Sidebar Styles */
        .sidebar {
            width: 250px;
            background-color: white;
            border-right: 1px solid white;
            display: flex;
            flex-direction: column;
            position: sticky;
            top: 0;
            height: 100vh;
        }

        /* Bootstrap-like utilities */
        .ps-3 {
            padding-left: 1rem !important;
        }

        .container-fluid {
            width: 100%;
            padding-right: 0;
            padding-left: 0;
            margin-right: auto;
            margin-left: auto;
        }

        .navbar {
            position: relative;
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            justify-content: space-between;
        }

        .flex-column {
            flex-direction: column !important;
        }

        .top-row {
            background-color: #ffffff;
            border-bottom: 1px solid white;
            height: 7rem;
            display: flex;
            align-items: center;
            min-height: 3.5rem;
        }

        .navbar-brand {
            font-size: 1.1rem;
            display: flex;
            align-items: center;
        }

        .brand-logo {
            height: 7rem;
            width: auto;
        }

        .nav-scrollable {
            flex: 1;
            overflow-y: auto;
        }

        nav {
            display: flex;
            flex-direction: column;
            height: 100%;
            padding: 1rem 0;
        }

        .nav-item {
            padding-left: 1rem;
            padding-right: 1rem;
        }

        .nav-item:first-of-type {
            padding-top: 1rem;
        }

        .nav-item:last-of-type {
            padding-bottom: 1rem;
        }

        .nav-item a {
            color: black;
            border-radius: 4px;
            height: 3rem;
            display: flex;
            align-items: center;
            line-height: 3rem;
            text-decoration: none;
            width: 100%;
            border: none;
            background: transparent;
            cursor: pointer;
            font-size: 1rem;
            padding: 0 1rem;
            transition: all 0.2s ease;
        }

        .nav-item a.active {
            background-color: rgb(231,231,231);
            color: black;
        }

        .nav-item a:hover {
            background-color: rgb(231,231,231);
            color: black;
        }

        .nav-icon {
            width: 1.25rem;
            height: 1.25rem;
            margin-right: 0.75rem;
            flex-shrink: 0;
        }

        .nav-spacer {
            flex: 1;
            min-height: 2rem;
        }

        /* Main Content */
        main {
            flex: 1;
            background-color: rgb(231, 231, 231);
        }

        .main-content {
            padding: 5rem;
        }

        /* Dashboard Page */
        .dashboard-page {
            min-height: 100vh;
        }

        .top-header {
            display: flex;
            justify-content: flex-end;
            align-items: flex-end;
            padding: 0.5rem 0;
            font-size: 1rem;
            margin-bottom: 1rem;
        }

        .page-tittle {
            margin: 0;
            text-align: right;
            font-size: 1rem;
            color: black;
        }

        /* Page Header */
        .page-header-wrapper {
            padding-bottom: 1rem;
            border-bottom: 1px solid #e8e8e8;
            margin-bottom: 2rem;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: -1rem;
            gap: 2rem;
        }

        .header-text {
            flex: 1;
        }

        .header-text .page-title {
            margin: 0 0 1rem 0;
            font-size: 28px;
            font-weight: 700;
            color: #000000;
        }

        .header-text .page-description {
            margin-top: 0;
            color: #6b6b6b;
            font-size: 14px;
        }

        .btn-add-user {
            display: flex;
            align-items: center;
            gap: 8px;
            padding: 12px 24px;
            background: #5aabea;
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 15px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            white-space: nowrap;
        }

        .btn-add-user:hover {
            background: #4a9bda;
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(90, 171, 234, 0.3);
        }

        .btn-icon {
            width: 18px;
            height: 18px;
            filter: brightness(0) invert(1);
        }

        /* Statistics Cards */
        .stats-container {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1rem;
            margin-bottom: 1.5rem;
        }

        .stat-card {
            background: white;
            border: 1px solid #e8e8e8;
            border-radius: 12px;
            padding: 1.25rem;
            text-align: center;
            transition: all 0.2s ease;
        }

        .stat-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        }

        .stat-value {
            font-size: 32px;
            font-weight: bold;
            color: #5aabea;
            margin-bottom: 0.5rem;
            transition: all 0.3s ease;
        }

        .stat-value.counting {
            transform: scale(1.1);
        }

        .stat-label {
            font-size: 14px;
            color: #6b6b6b;
            font-weight: 500;
        }

        /* Responsive */
        @media (max-width: 768px) {
            .page {
                flex-direction: column;
            }

            .sidebar {
                width: 100%;
                height: auto;
                position: relative;
            }

            .main-content {
                padding: 2rem;
            }

            .stats-container {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 640px) {
            .page-header {
                flex-direction: column;
                gap: 1rem;
            }

            .btn-add-user {
                width: 100%;
                justify-content: center;
            }
        }
    </style>
</head>
<body>
    <div class='page'>
        <!-- Sidebar -->
        <div class='sidebar'>
            <div class='top-row ps-3 navbar navbar-dark'>
                <div class='container-fluid'>
                    <div class='navbar-brand'>
                        <img src='{LOGO_SRC}' alt='EasyPeasy Logo' class='brand-logo' />
                    </div>
                </div>
            </div>
            
            <div class='nav-scrollable'>
                <nav class='flex-column'>
                    <div class='nav-item'>
                        <a class='active' href='/admin'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M104,40H56A16,16,0,0,0,40,56v48a16,16,0,0,0,16,16h48a16,16,0,0,0,16-16V56A16,16,0,0,0,104,40Zm0,64H56V56h48v48Zm96-64H152a16,16,0,0,0-16,16v48a16,16,0,0,0,16,16h48a16,16,0,0,0,16-16V56A16,16,0,0,0,200,40Zm0,64H152V56h48v48Zm-96,32H56a16,16,0,0,0-16,16v48a16,16,0,0,0,16,16h48a16,16,0,0,0,16-16V152A16,16,0,0,0,104,136Zm0,64H56V152h48v48Zm96-64H152a16,16,0,0,0-16,16v48a16,16,0,0,0,16,16h48a16,16,0,0,0,16-16V152A16,16,0,0,0,200,136Zm0,64H152V152h48v48Z' />
                            </svg>
                            Dashboard
                        </a>
                    </div>
                    <div class='nav-item'>
                        <a href='/admin/users'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M117.25,157.92a60,60,0,1,0-66.5,0A95.83,95.83,0,0,0,3.53,195.63a8,8,0,1,0,13.4,8.74,80,80,0,0,1,134.14,0,8,8,0,0,0,13.4-8.74A95.83,95.83,0,0,0,117.25,157.92ZM40,108a44,44,0,1,1,44,44A44.05,44.05,0,0,1,40,108Zm210.14,98.7a8,8,0,0,1-11.07-2.33A79.83,79.83,0,0,0,172,168a8,8,0,0,1,0-16,44,44,0,1,0-16.34-84.87,8,8,0,1,1-5.94-14.85,60,60,0,0,1,55.53,105.64,95.83,95.83,0,0,1,47.22,37.71A8,8,0,0,1,250.14,206.7Z' />
                            </svg>
                            User Accounts
                        </a>
                    </div>
                    <div class='nav-item'>
                        <a href='/admin/devices'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M224,72H208V64a24,24,0,0,0-24-24H40A24,24,0,0,0,16,64v96a24,24,0,0,0,24,24H152v8a24,24,0,0,0,24,24h48a24,24,0,0,0,24-24V96A24,24,0,0,0,224,72ZM40,168a8,8,0,0,1-8-8V64a8,8,0,0,1,8-8H184a8,8,0,0,1,8,8v8H176a24,24,0,0,0-24,24v72Zm192,24a8,8,0,0,1-8,8H176a8,8,0,0,1-8-8V96a8,8,0,0,1,8-8h48a8,8,0,0,1,8,8Zm-96,16a8,8,0,0,1-8,8H88a8,8,0,0,1,0-16h40A8,8,0,0,1,136,208Zm80-96a8,8,0,0,1-8,8H192a8,8,0,0,1,0-16h16A8,8,0,0,1,216,112Z' />
                            </svg>
                            Devices
                        </a>
                    </div>
                    <div class='nav-item'>
                        <a href='/admin/network'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M247.31,124.76c-.35-.79-8.82-19.58-27.65-38.41C194.57,61.26,162.88,48,128,48S61.43,61.26,36.34,86.35C17.51,105.18,9,124,8.69,124.76a8,8,0,0,0,0,6.5c.35.79,8.82,19.57,27.65,38.4C61.43,194.74,93.12,208,128,208s66.57-13.26,91.66-38.34c18.83-18.83,27.3-37.61,27.65-38.4A8,8,0,0,0,247.31,124.76ZM128,192c-30.78,0-57.67-11.19-79.93-33.25A133.47,133.47,0,0,1,25,128,133.33,133.33,0,0,1,48.07,97.25C70.33,75.19,97.22,64,128,64s57.67,11.19,79.93,33.25A133.46,133.46,0,0,1,231.05,128C223.84,141.46,192.43,192,128,192Zm0-112a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Z' />
                                <circle cx='128' cy='128' r='24' />
                            </svg>
                            Network Control
                        </a>
                    </div>
                    <div class='nav-item'>
                        <a href='/admin/settings'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M128,80a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Zm88-29.84q.06-2.16,0-4.32l14.92-18.64a8,8,0,0,0,1.48-7.06,107.21,107.21,0,0,0-10.88-26.25,8,8,0,0,0-6-3.93l-23.72-2.64q-1.48-1.56-3-3L186,40.54a8,8,0,0,0-3.94-6,107.71,107.71,0,0,0-26.25-10.87,8,8,0,0,0-7.06,1.49L130.16,40Q128,40,125.84,40L107.2,25.11a8,8,0,0,0-7.06-1.48A107.6,107.6,0,0,0,73.89,34.51a8,8,0,0,0-3.93,6L67.32,64.27q-1.56,1.49-3,3L40.54,70a8,8,0,0,0-6,3.94,107.71,107.71,0,0,0-10.87,26.25,8,8,0,0,0,1.49,7.06L40,125.84Q40,128,40,130.16L25.11,148.8a8,8,0,0,0-1.48,7.06,107.21,107.21,0,0,0,10.88,26.25,8,8,0,0,0,6,3.93l23.72,2.64q1.49,1.56,3,3L70,215.46a8,8,0,0,0,3.94,6,107.71,107.71,0,0,0,26.25,10.87,8,8,0,0,0,7.06-1.49L125.84,216q2.16.06,4.32,0l18.64,14.92a8,8,0,0,0,7.06,1.48,107.21,107.21,0,0,0,26.25-10.88,8,8,0,0,0,3.93-6l2.64-23.72q1.56-1.48,3-3L215.46,186a8,8,0,0,0,6-3.94,107.71,107.71,0,0,0,10.87-26.25,8,8,0,0,0-1.49-7.06Zm-16.1-6.5a73.93,73.93,0,0,1,0,8.68,8,8,0,0,0,1.74,5.48l14.19,17.73a91.57,91.57,0,0,1-6.23,15L187,173.11a8,8,0,0,0-5.1,2.64,74.11,74.11,0,0,1-6.14,6.14,8,8,0,0,0-2.64,5.1l-2.51,22.58a91.32,91.32,0,0,1-15,6.23l-17.74-14.19a8,8,0,0,0-5-1.75h-.48a73.93,73.93,0,0,1-8.68,0,8,8,0,0,0-5.48,1.74L100.45,215.8a91.57,91.57,0,0,1-15-6.23L82.89,187a8,8,0,0,0-2.64-5.1,74.11,74.11,0,0,1-6.14-6.14,8,8,0,0,0-5.1-2.64L46.43,170.6a91.32,91.32,0,0,1-6.23-15l14.19-17.74a8,8,0,0,0,1.74-5.48,73.93,73.93,0,0,1,0-8.68,8,8,0,0,0-1.74-5.48L40.2,100.45a91.57,91.57,0,0,1,6.23-15L69,82.89a8,8,0,0,0,5.1-2.64,74.11,74.11,0,0,1,6.14-6.14A8,8,0,0,0,82.89,69L85.4,46.43a91.32,91.32,0,0,1,15-6.23l17.74,14.19a8,8,0,0,0,5.48,1.74,73.93,73.93,0,0,1,8.68,0,8,8,0,0,0,5.48-1.74L155.55,40.2a91.57,91.57,0,0,1,15,6.23L173.11,69a8,8,0,0,0,2.64,5.1,74.11,74.11,0,0,1,6.14,6.14,8,8,0,0,0,5.1,2.64l22.58,2.51a91.32,91.32,0,0,1,6.23,15l-14.19,17.74A8,8,0,0,0,199.87,123.66Z' />
                            </svg>
                            Settings
                        </a>
                    </div>
                    <div class='nav-spacer'></div>
                </nav>
            </div>
        </div>

        <!-- Main Content -->
        <main>
            <div class='main-content'>
                <div class='dashboard-page'>
                    <div class='top-header'>
                        <h1 class='page-tittle'>Administrator System</h1>
                    </div>

                    <!-- Header -->
                    <div class='page-header-wrapper'>
                        <div class='page-header'>
                            <div class='header-text'>
                                <h1 class='page-title'>Dashboard</h1>
                                <p class='page-description'>Welcome to your Network Control System</p>
                            </div>
                            <button class='btn-add-user' onclick='window.location.href=&quot;/admin/users&quot;'>
                                <svg class='btn-icon' viewBox='0 0 256 256' fill='currentColor'>
                                    <path d='M228,128a12,12,0,0,1-12,12H140v76a12,12,0,0,1-24,0V140H40a12,12,0,0,1,0-24h76V40a12,12,0,0,1,24,0v76h76A12,12,0,0,1,228,128Z' />
                                </svg>
                                Add User
                            </button>
                        </div>
                    </div>

                    <!-- Statistics Cards -->
                    <div class='stats-container'>
                        <div class='stat-card'>
                            <div class='stat-value' id='users-connected'>{USERS_CONNECTED}</div>
                            <div class='stat-label'>Users connected</div>
                        </div>
                        <div class='stat-card'>
                            <div class='stat-value' id='active-sessions'>{ACTIVE_SESSIONS}</div>
                            <div class='stat-label'>Active sessions</div>
                        </div>
                        <div class='stat-card'>
                            <div class='stat-value' id='registered-users'>{REGISTERED_USERS}</div>
                            <div class='stat-label'>Registered users</div>
                        </div>
                    </div>
                </div>
            </div>
        </main>
    </div>

    <script>
        const API_BASE = 'http://localhost:8080/api';

        // Counter animation for statistics
        function animateCounter(element, target) {
            const duration = 1000; // 1 second
            const start = 0;
            const startTime = performance.now();

            function update(currentTime) {
                const elapsed = currentTime - startTime;
                const progress = Math.min(elapsed / duration, 1);
                
                // Easing function (easeOutCubic)
                const easeProgress = 1 - Math.pow(1 - progress, 3);
                const current = Math.floor(start + (target - start) * easeProgress);
                
                element.textContent = current;
                element.classList.add('counting');
                
                if (progress < 1) {
                    requestAnimationFrame(update);
                } else {
                    element.classList.remove('counting');
                }
            }
            
            requestAnimationFrame(update);
        }

        // Load statistics from API
        async function loadStatistics() {
            try {
                const [devicesResponse, sessionsResponse, usersResponse] = await Promise.all([
                    fetch(`${API_BASE}/device`),
                    fetch(`${API_BASE}/session`),
                    fetch(`${API_BASE}/users`)
                ]);

                const devices = devicesResponse.ok ? await devicesResponse.json() : [];
                const sessions = sessionsResponse.ok ? await sessionsResponse.json() : [];
                const users = usersResponse.ok ? await usersResponse.json() : [];

                const usersConnected = devices.length;
                const activeSessions = sessions.length;
                const registeredUsers = users.length;

                // Update elements
                const usersConnectedEl = document.getElementById('users-connected');
                const activeSessionsEl = document.getElementById('active-sessions');
                const registeredUsersEl = document.getElementById('registered-users');

                // Animate counters
                setTimeout(() => animateCounter(usersConnectedEl, usersConnected), 100);
                setTimeout(() => animateCounter(activeSessionsEl, activeSessions), 200);
                setTimeout(() => animateCounter(registeredUsersEl, registeredUsers), 300);
            } catch (error) {
                console.error('Error loading statistics:', error);
                // Use default values from HTML if API fails
                const usersConnected = document.getElementById('users-connected');
                const activeSessions = document.getElementById('active-sessions');
                const registeredUsers = document.getElementById('registered-users');
                
                const usersTarget = parseInt(usersConnected.textContent) || 0;
                const sessionsTarget = parseInt(activeSessions.textContent) || 0;
                const registeredTarget = parseInt(registeredUsers.textContent) || 0;
                
                setTimeout(() => animateCounter(usersConnected, usersTarget), 100);
                setTimeout(() => animateCounter(activeSessions, sessionsTarget), 200);
                setTimeout(() => animateCounter(registeredUsers, registeredTarget), 300);
            }
        }

        // Initialize when page loads
        window.addEventListener('DOMContentLoaded', loadStatistics);

        // Auto-refresh stats every 30 seconds
        setInterval(loadStatistics, 30000);
    </script>
</body>
</html>";

    /// <summary>
    /// Generates the admin dashboard page with statistics
    /// </summary>
    /// <param name="usersConnected">Number of currently connected users</param>
    /// <param name="activeSessions">Number of active sessions</param>
    /// <param name="registeredUsers">Total number of registered users</param>
    /// <param name="logoSrc">Optional custom logo source (defaults to embedded logo)</param>
    /// <returns>Complete HTML string for the dashboard page</returns>
    public static string GenerateDashboard(
        int usersConnected = 0,
        int activeSessions = 0,
        int registeredUsers = 0,
        string? logoSrc = null)
    {
        // Use embedded logo by default
        string finalLogoSrc = logoSrc ?? DashboardPageConst.LOGO_DATA_URI;
        
        return HTML_TEMPLATE
            .Replace("{LOGO_SRC}", finalLogoSrc)
            .Replace("{USERS_CONNECTED}", usersConnected.ToString())
            .Replace("{ACTIVE_SESSIONS}", activeSessions.ToString())
            .Replace("{REGISTERED_USERS}", registeredUsers.ToString());
    }
}
