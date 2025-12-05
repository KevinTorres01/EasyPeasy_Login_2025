namespace EasyPeasy_Login.Server.HtmlPages.Admin;

public static class NetworkControlPage
{
    private const string HTML_TEMPLATE = @"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Network Control - EasyPeasy Login</title>
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

        /* Network Control Page */
        .network-control-page {
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

        /* Network Status Badge */
        .network-status {
            display: flex;
            align-items: center;
            gap: 8px;
            padding: 8px 16px;
            border-radius: 20px;
            font-weight: 500;
            font-size: 14px;
        }

        .network-status.status-active {
            background-color: #80d8d0;
            color: rgb(32,102,94);
        }

        .network-status.status-inactive {
            background-color: rgb(238,190,196);
            color: rgb(187,49,62);
        }

        .status-dot {
            width: 10px;
            height: 10px;
            border-radius: 50%;
            animation: pulse 2s infinite;
        }

        .status-active .status-dot {
            background-color: rgb(32,102,94);
        }

        .status-inactive .status-dot {
            background-color: rgb(187,49,62);
            animation: none;
        }

        @keyframes pulse {
            0%, 100% { opacity: 1; transform: scale(1); }
            50% { opacity: 0.6; transform: scale(1.1); }
        }

        /* Network Content Layout */
        .network-content {
            display: grid;
            grid-template-columns: 1fr 1.5fr;
            gap: 24px;
            margin-bottom: 24px;
        }

        /* Control Panel & Config Panel */
        .control-panel,
        .config-panel {
            background: white;
            border-radius: 12px;
            padding: 24px;
            border: 1px solid #e8e8e8;
        }

        .panel-section {
            margin-bottom: 24px;
        }

        .panel-section:last-child {
            margin-bottom: 0;
        }

        .section-title {
            display: flex;
            align-items: center;
            gap: 10px;
            font-size: 16px;
            font-weight: 600;
            color: black;
            margin-bottom: 16px;
        }

        .section-icon {
            width: 20px;
            height: 20px;
            color: black;
        }

        /* Action Buttons */
        .action-buttons {
            display: flex;
            gap: 12px;
        }

        .btn-action {
            flex: 1;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 14px 20px;
            border: none;
            border-radius: 10px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            opacity: 1 !important;
        }

        .btn-action .btn-icon {
            width: 20px;
            height: 20px;
            opacity: 1 !important;
        }

        .btn-start {
            background: #80d8d0 !important;
            color: white !important;
        }

        .btn-start:hover:not(:disabled) {
            background: rgb(60,190,179) !important;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(32,102,94,0.3);
        }

        .btn-stop {
            background: rgb(231, 73, 124) !important;
            color: white !important;
        }

        .btn-stop:hover:not(:disabled) {
            background: rgb(212, 28,89) !important;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(132,17,55,0.3);
        }

        .btn-action:disabled,
        .btn-action.disabled {
            opacity: 0.5;
            cursor: not-allowed;
            transform: none !important;
            box-shadow: none !important;
        }

        /* Spinner */
        .spinner {
            width: 18px;
            height: 18px;
            border: 2px solid rgba(255, 255, 255, 0.3);
            border-top-color: white;
            border-radius: 50%;
            animation: spin 0.8s linear infinite;
        }

        @keyframes spin {
            to { transform: rotate(360deg); }
        }

        /* Runtime Info */
        .runtime-info {
            background: rgba(99, 102, 241, 0.1);
            border-radius: 10px;
            padding: 16px;
            border: 1px solid rgba(99, 102, 241, 0.2);
        }

        .info-grid {
            display: flex;
            flex-direction: column;
            gap: 12px;
        }

        .info-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .info-label {
            color: #6b6b6b;
            font-size: 13px;
        }

        .info-value {
            color: black;
            font-size: 13px;
            font-weight: 500;
        }

        .info-value.url {
            color: #5aabea;
            font-family: monospace;
            font-size: 12px;
        }

        .info-value.vpn-active {
            color: #22c55e;
        }

        /* Config Panel Header */
        .panel-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 16px;
        }

        .panel-header .section-title {
            margin-bottom: 0;
            color: black;
        }

        .btn-reset {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 36px;
            height: 36px;
            border: none;
            border-radius: 8px;
            background: #f5f5f5;
            color: #6b6b6b;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-reset:hover:not(:disabled) {
            background: #e8e8e8;
            color: black;
        }

        .btn-reset:disabled {
            opacity: 0.4;
            cursor: not-allowed;
        }

        .btn-reset svg {
            width: 18px;
            height: 18px;
        }

        /* Config Locked Notice */
        .config-locked-notice {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 12px 16px;
            background: rgba(234, 179, 8, 0.15);
            border: 1px solid rgba(234, 179, 8, 0.3);
            border-radius: 8px;
            margin-bottom: 20px;
            color: #92400e;
            font-size: 13px;
        }

        .config-locked-notice svg {
            width: 18px;
            height: 18px;
            flex-shrink: 0;
            color: #eab308;
        }

        /* Config Form */
        .config-form {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
        }

        .form-group {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .form-group label {
            font-size: 13px;
            font-weight: 500;
            color: black;
        }

        .form-group input {
            padding: 10px 14px;
            border: 1px solid #e8e8e8;
            border-radius: 8px;
            background: white;
            color: black;
            font-size: 14px;
            transition: border-color 0.2s ease;
        }

        .form-group input:focus {
            outline: none;
            border-color: #5aabea;
        }

        .form-group input:disabled {
            background: #f5f5f5;
            cursor: not-allowed;
        }

        .form-hint {
            font-size: 11px;
            color: #6b6b6b;
        }

        /* Password Input */
        .password-input {
            position: relative;
            display: flex;
        }

        .password-input input {
            flex: 1;
            padding-right: 44px;
        }

        .btn-toggle-password {
            position: absolute;
            right: 8px;
            top: 50%;
            transform: translateY(-50%);
            width: 32px;
            height: 32px;
            display: flex;
            align-items: center;
            justify-content: center;
            border: none;
            background: transparent;
            color: #6b6b6b;
            cursor: pointer;
            border-radius: 6px;
            transition: all 0.2s ease;
        }

        .btn-toggle-password:hover {
            color: black;
            background: #f5f5f5;
        }

        .btn-toggle-password svg {
            width: 18px;
            height: 18px;
        }

        /* Responsive */
        @media (max-width: 1024px) {
            .network-content {
                grid-template-columns: 1fr;
            }
        }

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

            .form-row {
                grid-template-columns: 1fr;
            }
            
            .action-buttons {
                flex-direction: column;
            }
        }

        /* Log Console Styles */
        .log-console {
            background: #2a2a2a;
            border-radius: 16px;
            border: 1px solid #404040;
            overflow: hidden;
            transition: all 0.3s ease;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
            margin-top: 24px;
        }

        .log-console.collapsed {
            max-height: 56px;
        }

        .log-console.expanded {
            max-height: 400px;
        }

        /* Console Header */
        .console-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 14px 20px;
            background: #2a2a2a;
            border-bottom: 1px solid #404040;
            cursor: pointer;
            user-select: none;
            transition: background 0.2s ease;
        }

        .console-header:hover {
            background: #333333;
        }

        .console-title {
            display: flex;
            align-items: center;
            gap: 10px;
            color: #e0e0e0;
            font-weight: 600;
            font-size: 14px;
        }

        .console-icon {
            width: 20px;
            height: 20px;
            color: #5aabea;
            flex-shrink: 0;
        }

        .log-count {
            font-size: 12px;
            color: #808080;
            font-weight: 400;
        }

        .console-actions {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .btn-console-action {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 32px;
            height: 32px;
            border: none;
            border-radius: 6px;
            background: transparent;
            color: #808080;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-console-action:hover {
            background: rgba(255, 255, 255, 0.1);
            color: #e0e0e0;
        }

        .btn-console-action svg {
            width: 18px;
            height: 18px;
        }

        .btn-console-action svg.active {
            color: #5aabea;
        }

        .expand-icon {
            font-size: 12px;
            color: #808080;
            margin-left: 4px;
        }

        /* Console Body */
        .console-body {
            max-height: 320px;
            overflow-y: auto;
            padding: 12px 16px;
            font-family: 'JetBrains Mono', 'Fira Code', 'Consolas', monospace;
            font-size: 12px;
            line-height: 1.6;
            background: #2a2a2a;
        }

        .console-body::-webkit-scrollbar {
            width: 8px;
        }

        .console-body::-webkit-scrollbar-track {
            background: rgba(0, 0, 0, 0.2);
        }

        .console-body::-webkit-scrollbar-thumb {
            background: rgba(255, 255, 255, 0.15);
            border-radius: 4px;
        }

        .console-body::-webkit-scrollbar-thumb:hover {
            background: rgba(255, 255, 255, 0.25);
        }

        /* Console Empty State */
        .console-empty {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 40px 20px;
            color: #808080;
            font-style: italic;
        }

        /* Log Entries */
        .log-entry {
            display: flex;
            gap: 8px;
            padding: 4px 0;
            border-bottom: 1px solid rgba(255, 255, 255, 0.05);
        }

        .log-entry:last-child {
            border-bottom: none;
        }

        .log-timestamp {
            color: #6b7280;
            flex-shrink: 0;
        }

        .log-level {
            flex-shrink: 0;
            font-weight: 600;
            min-width: 70px;
        }

        .log-message {
            color: #d0d0d0;
            word-break: break-word;
        }

        /* Log Level Colors */
        .log-info .log-level {
            color: #3b82f6;
        }

        .log-info .log-message {
            color: #93c5fd;
        }

        .log-warning .log-level {
            color: #eab308;
        }

        .log-warning .log-message {
            color: #fde047;
        }

        .log-error .log-level {
            color: #ef4444;
        }

        .log-error .log-message {
            color: #fca5a5;
        }

        /* Animation for new logs */
        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(-4px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .log-entry {
            animation: fadeIn 0.2s ease;
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
                        <a href='/admin'>
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
                        <a class='active' href='/admin/network'>
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
                <div class='network-control-page'>
                    <div class='top-header'>
                        <h1 class='page-tittle'>Administrator System</h1>
                    </div>

                    <!-- Header -->
                    <div class='page-header-wrapper'>
                        <div class='page-header'>
                            <div class='header-text'>
                                <h1 class='page-title'>Network Control</h1>
                                <p class='page-description'>Configure and manage the Captive Portal Access Point</p>
                            </div>
                            <div class='network-status {STATUS_CLASS}'>
                                <span class='status-dot'></span>
                                <span class='status-text'>{STATUS_TEXT}</span>
                            </div>
                        </div>
                    </div>

                    <div class='network-content'>
                        <!-- Control Panel -->
                        <div class='control-panel'>
                            <div class='panel-section'>
                                <h3 class='section-title'>
                                    <svg class='section-icon' viewBox='0 0 256 256' fill='currentColor'>
                                        <path d='M247.31,124.76c-.35-.79-8.82-19.58-27.65-38.41C194.57,61.26,162.88,48,128,48S61.43,61.26,36.34,86.35C17.51,105.18,9,124,8.69,124.76a8,8,0,0,0,0,6.5c.35.79,8.82,19.57,27.65,38.4C61.43,194.74,93.12,208,128,208s66.57-13.26,91.66-38.34c18.83-18.83,27.3-37.61,27.65-38.4A8,8,0,0,0,247.31,124.76ZM128,192c-30.78,0-57.67-11.19-79.93-33.25A133.47,133.47,0,0,1,25,128,133.33,133.33,0,0,1,48.07,97.25C70.33,75.19,97.22,64,128,64s57.67,11.19,79.93,33.25A133.46,133.46,0,0,1,231.05,128C223.84,141.46,192.43,192,128,192Zm0-112a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Z'/>
                                    </svg>
                                    Network Actions
                                </h3>
                                <div class='action-buttons'>
                                    <button class='btn-action btn-start' {START_DISABLED} onclick='alert(&quot;Start Network - This would call your C# backend&quot;)'>
                                        <svg class='btn-icon' viewBox='0 0 256 256' fill='currentColor'>
                                            <path d='M232,128A104,104,0,1,1,128,24,104.11,104.11,0,0,1,232,128ZM128,48a80,80,0,1,0,80,80A80.09,80.09,0,0,0,128,48Zm36.44,77.66-48-32A8,8,0,0,0,104,100v64a8,8,0,0,0,12.44,6.66l48-32a8,8,0,0,0,0-13.32Z'/>
                                        </svg>
                                        <span>Start Network</span>
                                    </button>
                                    <button class='btn-action btn-stop' {STOP_DISABLED} onclick='alert(&quot;Stop Network - This would call your C# backend&quot;)'>
                                        <svg class='btn-icon' viewBox='0 0 256 256' fill='currentColor'>
                                            <path d='M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24Zm0,192a88,88,0,1,1,88-88A88.1,88.1,0,0,1,128,216Zm24-120v64a8,8,0,0,1-16,0V96a8,8,0,0,1,16,0Zm-40,0v64a8,8,0,0,1-16,0V96a8,8,0,0,1,16,0Z'/>
                                        </svg>
                                        <span>Stop Network</span>
                                    </button>
                                </div>
                            </div>

                            {RUNTIME_INFO}
                        </div>

                        <!-- Configuration Panel -->
                        <div class='config-panel'>
                            <div class='panel-header'>
                                <h3 class='section-title'>
                                    <svg class='section-icon' viewBox='0 0 256 256' fill='currentColor'>
                                        <path d='M128,80a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Zm88-29.84q.06-2.16,0-4.32l14.92-18.64a8,8,0,0,0,1.48-7.06,107.21,107.21,0,0,0-10.88-26.25,8,8,0,0,0-6-3.93l-23.72-2.64q-1.48-1.56-3-3L186,40.54a8,8,0,0,0-3.94-6,107.71,107.71,0,0,0-26.25-10.87,8,8,0,0,0-7.06,1.49L130.16,40Q128,40,125.84,40L107.2,25.11a8,8,0,0,0-7.06-1.48A107.6,107.6,0,0,0,73.89,34.51a8,8,0,0,0-3.93,6L67.32,64.27q-1.56,1.49-3,3L40.54,70a8,8,0,0,0-6,3.94,107.71,107.71,0,0,0-10.87,26.25,8,8,0,0,0,1.49,7.06L40,125.84Q40,128,40,130.16L25.11,148.8a8,8,0,0,0-1.48,7.06,107.21,107.21,0,0,0,10.88,26.25,8,8,0,0,0,6,3.93l23.72,2.64q1.49,1.56,3,3L70,215.46a8,8,0,0,0,3.94,6,107.71,107.71,0,0,0,26.25,10.87,8,8,0,0,0,7.06-1.49L125.84,216q2.16.06,4.32,0l18.64,14.92a8,8,0,0,0,7.06,1.48,107.21,107.21,0,0,0,26.25-10.88,8,8,0,0,0,3.93-6l2.64-23.72q1.56-1.48,3-3L215.46,186a8,8,0,0,0,6-3.94,107.71,107.71,0,0,0,10.87-26.25,8,8,0,0,0-1.49-7.06Zm-16.1-6.5a73.93,73.93,0,0,1,0,8.68,8,8,0,0,0,1.74,5.48l14.19,17.73a91.57,91.57,0,0,1-6.23,15L187,173.11a8,8,0,0,0-5.1,2.64,74.11,74.11,0,0,1-6.14,6.14,8,8,0,0,0-2.64,5.1l-2.51,22.58a91.32,91.32,0,0,1-15,6.23l-17.74-14.19a8,8,0,0,0-5-1.75h-.48a73.93,73.93,0,0,1-8.68,0,8,8,0,0,0-5.48,1.74L100.45,215.8a91.57,91.57,0,0,1-15-6.23L82.89,187a8,8,0,0,0-2.64-5.1,74.11,74.11,0,0,1-6.14-6.14,8,8,0,0,0-5.1-2.64L46.43,170.6a91.32,91.32,0,0,1-6.23-15l14.19-17.74a8,8,0,0,0,1.74-5.48,73.93,73.93,0,0,1,0-8.68,8,8,0,0,0-1.74-5.48L40.2,100.45a91.57,91.57,0,0,1,6.23-15L69,82.89a8,8,0,0,0,5.1-2.64,74.11,74.11,0,0,1,6.14-6.14A8,8,0,0,0,82.89,69L85.4,46.43a91.32,91.32,0,0,1,15-6.23l17.74,14.19a8,8,0,0,0,5.48,1.74,73.93,73.93,0,0,1,8.68,0,8,8,0,0,0,5.48-1.74L155.55,40.2a91.57,91.57,0,0,1,15,6.23L173.11,69a8,8,0,0,0,2.64,5.1,74.11,74.11,0,0,1,6.14,6.14,8,8,0,0,0,5.1,2.64l22.58,2.51a91.32,91.32,0,0,1,6.23,15l-14.19,17.74A8,8,0,0,0,199.87,123.66Z'/>
                                    </svg>
                                    Network Configuration
                                </h3>
                                <button class='btn-reset' title='Reset to defaults' {RESET_DISABLED}>
                                    <svg viewBox='0 0 256 256' fill='currentColor'>
                                        <path d='M224,48V96a8,8,0,0,1-8,8H168a8,8,0,0,1,0-16h28.69L182.06,73.37a79.56,79.56,0,0,0-56.13-23.43h-.45A79.52,79.52,0,0,0,69.59,72.71,8,8,0,0,1,58.41,61.27a96,96,0,0,1,135,.79L208,76.69V48a8,8,0,0,1,16,0ZM186.41,183.29a80,80,0,0,1-112.47-.78L59.31,168H88a8,8,0,0,0,0-16H40a8,8,0,0,0-8,8v48a8,8,0,0,0,16,0V179.31l14.63,14.63A95.43,95.43,0,0,0,130,222.06h.53a95.36,95.36,0,0,0,67.07-27.33,8,8,0,0,0-11.18-11.44Z'/>
                                    </svg>
                                </button>
                            </div>
                            
                            {CONFIG_LOCKED}
                            
                            <div class='config-form'>
                                <div class='form-row'>
                                    <div class='form-group'>
                                        <label for='interface'>WiFi Interface</label>
                                        <input type='text' id='interface' value='{INTERFACE}' placeholder='e.g., wlan0' {CONFIG_DISABLED} />
                                        <span class='form-hint'>The wireless interface to use for the Access Point</span>
                                    </div>
                                    <div class='form-group'>
                                        <label for='ssid'>Network Name (SSID)</label>
                                        <input type='text' id='ssid' value='{SSID}' placeholder='e.g., MyNetwork' {CONFIG_DISABLED} />
                                        <span class='form-hint'>The name users will see when connecting</span>
                                    </div>
                                </div>
                                
                                <div class='form-row'>
                                    <div class='form-group'>
                                        <label for='password'>WiFi Password</label>
                                        <div class='password-input'>
                                            <input type='password' id='password' value='{PASSWORD}' placeholder='Minimum 8 characters' {CONFIG_DISABLED} />
                                            <button type='button' class='btn-toggle-password' onclick='togglePassword()'>
                                                <svg id='eye-icon' viewBox='0 0 256 256' fill='currentColor'>
                                                    <path d='M247.31,124.76c-.35-.79-8.82-19.58-27.65-38.41C194.57,61.26,162.88,48,128,48S61.43,61.26,36.34,86.35C17.51,105.18,9,124,8.69,124.76a8,8,0,0,0,0,6.5c.35.79,8.82,19.57,27.65,38.4C61.43,194.74,93.12,208,128,208s66.57-13.26,91.66-38.34c18.83-18.83,27.3-37.61,27.65-38.4A8,8,0,0,0,247.31,124.76ZM128,192c-30.78,0-57.67-11.19-79.93-33.25A133.47,133.47,0,0,1,25,128,133.33,133.33,0,0,1,48.07,97.25C70.33,75.19,97.22,64,128,64s57.67,11.19,79.93,33.25A133.46,133.46,0,0,1,231.05,128C223.84,141.46,192.43,192,128,192Zm0-112a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Z'/>
                                                </svg>
                                            </button>
                                        </div>
                                        <span class='form-hint'>WPA2 password for the network</span>
                                    </div>
                                    <div class='form-group'>
                                        <label for='port'>Portal Port</label>
                                        <input type='number' id='port' value='{PORT}' min='1' max='65535' {CONFIG_DISABLED} />
                                        <span class='form-hint'>HTTP port for the captive portal</span>
                                    </div>
                                </div>
                                
                                <div class='form-row'>
                                    <div class='form-group'>
                                        <label for='gateway'>Gateway IP</label>
                                        <input type='text' id='gateway' value='{GATEWAY}' placeholder='e.g., 192.168.1.1' {CONFIG_DISABLED} />
                                        <span class='form-hint'>The IP address of this device on the AP network</span>
                                    </div>
                                    <div class='form-group'>
                                        <label for='dhcp'>DHCP Range</label>
                                        <input type='text' id='dhcp' value='{DHCP_RANGE}' placeholder='e.g., 192.168.1.50,192.168.1.150' {CONFIG_DISABLED} />
                                        <span class='form-hint'>Range of IPs to assign to connected devices</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Log Console -->
                    <div class='log-console {CONSOLE_STATE}' id='logConsole'>
                        <div class='console-header' onclick='toggleConsole()'>
                            <div class='console-title'>
                                <svg class='console-icon' viewBox='0 0 256 256' fill='currentColor'>
                                    <path d='M224,144v56a16,16,0,0,1-16,16H48a16,16,0,0,1-16-16V144a8,8,0,0,1,16,0v56H208V144a8,8,0,0,1,16,0ZM92.66,73.66,120,46.63V144a8,8,0,0,0,16,0V46.63l27.34,27.33a8,8,0,0,0,11.32-11.32l-40-40a8,8,0,0,0-11.32,0l-40,40A8,8,0,0,0,92.66,73.66Z'/>
                                </svg>
                                <span>Console Output</span>
                                <span class='log-count' id='logCount'>(0 logs)</span>
                            </div>
                            <div class='console-actions'>
                                <button class='btn-console-action' onclick='clearLogs(event)' title='Clear logs'>
                                    <svg viewBox='0 0 256 256' fill='currentColor'>
                                        <path d='M216,48H176V40a24,24,0,0,0-24-24H104A24,24,0,0,0,80,40v8H40a8,8,0,0,0,0,16h8V208a16,16,0,0,0,16,16H192a16,16,0,0,0,16-16V64h8a8,8,0,0,0,0-16ZM96,40a8,8,0,0,1,8-8h48a8,8,0,0,1,8,8v8H96Zm96,168H64V64H192ZM112,104v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Zm48,0v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Z'/>
                                    </svg>
                                </button>
                                <button class='btn-console-action' onclick='toggleAutoScroll(event)' title='Toggle auto-scroll' id='autoScrollBtn'>
                                    <svg viewBox='0 0 256 256' fill='currentColor' class='active' id='autoScrollIcon'>
                                        <path d='M205.66,149.66l-72,72a8,8,0,0,1-11.32,0l-72-72a8,8,0,0,1,11.32-11.32L120,196.69V40a8,8,0,0,1,16,0V196.69l58.34-58.35a8,8,0,0,1,11.32,11.32Z'/>
                                    </svg>
                                </button>
                                <span class='expand-icon' id='expandIcon'>▼</span>
                            </div>
                        </div>
                        
                        <div class='console-body' id='consoleBody'>
                            {CONSOLE_LOGS}
                        </div>
                    </div>
                </div>
            </div>
        </main>
    </div>

    <script>
        // Console functionality
        let consoleExpanded = {CONSOLE_EXPANDED};
        let autoScroll = true;

        function toggleConsole() {{
            const console = document.getElementById('logConsole');
            const expandIcon = document.getElementById('expandIcon');
            consoleExpanded = !consoleExpanded;
            
            if (consoleExpanded) {{
                console.classList.add('expanded');
                console.classList.remove('collapsed');
                expandIcon.textContent = '▼';
            }} else {{
                console.classList.remove('expanded');
                console.classList.add('collapsed');
                expandIcon.textContent = '▲';
            }}
        }}

        function clearLogs(event) {{
            event.stopPropagation();
            const consoleBody = document.getElementById('consoleBody');
            const logCount = document.getElementById('logCount');
            
            consoleBody.innerHTML = '<div class=""empty-console"">No logs available</div>';
            logCount.textContent = '(0 logs)';
        }}

        function toggleAutoScroll(event) {{
            event.stopPropagation();
            autoScroll = !autoScroll;
            const autoScrollBtn = document.getElementById('autoScrollBtn');
            const autoScrollIcon = document.getElementById('autoScrollIcon');
            
            if (autoScroll) {{
                autoScrollBtn.classList.add('active');
                scrollToBottom();
            }} else {{
                autoScrollBtn.classList.remove('active');
            }}
        }}

        function scrollToBottom() {{
            if (autoScroll) {{
                const consoleBody = document.getElementById('consoleBody');
                consoleBody.scrollTop = consoleBody.scrollHeight;
            }}
        }}

        function updateLogCount() {{
            const logCount = document.getElementById('logCount');
            const logs = document.querySelectorAll('.log-entry');
            logCount.textContent = `(${{logs.length}} logs)`;
        }}

        // Initialize log count on load
        window.addEventListener('load', () => {{
            updateLogCount();
            if (autoScroll) {{
                setTimeout(scrollToBottom, 100);
            }}
        }});
        
        function togglePassword() {
            const passwordInput = document.getElementById('password');
            const eyeIcon = document.getElementById('eye-icon');
            
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                eyeIcon.innerHTML = '<path d=&quot;M228,175a8,8,0,0,1-10.92-3l-19-33.2A123.23,123.23,0,0,1,162,155.46l5.87,35.22a8,8,0,0,1-6.58,9.21A8.4,8.4,0,0,1,160,200a8,8,0,0,1-7.88-6.69l-5.77-34.58a133.06,133.06,0,0,1-36.68,0l-5.77,34.58A8,8,0,0,1,96,200a8.4,8.4,0,0,1-1.32-.11,8,8,0,0,1-6.58-9.21L94,155.46a123.23,123.23,0,0,1-36.06-16.69L38.92,172A8,8,0,1,1,25.08,164l20-35a153.47,153.47,0,0,1-19.3-20A8,8,0,1,1,38.22,99a135.86,135.86,0,0,0,99.73,53h.1a135.86,135.86,0,0,0,99.73-53,8,8,0,1,1,12.46,10,153.47,153.47,0,0,1-19.3,20l20,35A8,8,0,0,1,228,175Z&quot;/>';
            } else {
                passwordInput.type = 'password';
                eyeIcon.innerHTML = '<path d=&quot;M247.31,124.76c-.35-.79-8.82-19.58-27.65-38.41C194.57,61.26,162.88,48,128,48S61.43,61.26,36.34,86.35C17.51,105.18,9,124,8.69,124.76a8,8,0,0,0,0,6.5c.35.79,8.82,19.57,27.65,38.4C61.43,194.74,93.12,208,128,208s66.57-13.26,91.66-38.34c18.83-18.83,27.3-37.61,27.65-38.4A8,8,0,0,0,247.31,124.76ZM128,192c-30.78,0-57.67-11.19-79.93-33.25A133.47,133.47,0,0,1,25,128,133.33,133.33,0,0,1,48.07,97.25C70.33,75.19,97.22,64,128,64s57.67,11.19,79.93,33.25A133.46,133.46,0,0,1,231.05,128C223.84,141.46,192.43,192,128,192Zm0-112a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Z&quot;/>';
            }
        }
    </script>
</body>
</html>";

    /// <summary>
    /// Generates the network control page with configuration
    /// </summary>
    public static string GenerateNetworkControl(
        bool isNetworkActive = false,
        string? upstreamInterface = null,
        bool isVpnInterface = false,
        string gatewayIp = "192.168.4.1",
        int defaultPort = 80,
        string wifiInterface = "wlan0",
        string ssid = "EasyPeasy WiFi",
        string password = "easypeasy123",
        string dhcpRange = "192.168.4.50,192.168.4.150",
        bool consoleExpanded = true,
        string[]? sampleLogs = null,
        string? logoSrc = null)
    {
        string finalLogoSrc = logoSrc ?? DashboardPageConst.LOGO_DATA_URI;
        
        string statusClass = isNetworkActive ? "status-active" : "status-inactive";
        string statusText = isNetworkActive ? "Network Active" : "Network Inactive";
        
        string startDisabled = isNetworkActive ? "disabled" : "";
        string stopDisabled = !isNetworkActive ? "disabled" : "";
        string resetDisabled = isNetworkActive ? "disabled" : "";
        string configDisabled = isNetworkActive ? "disabled" : "";
        
        // Console state
        string consoleState = consoleExpanded ? "expanded" : "collapsed";
        string consoleExpandedJs = consoleExpanded ? "true" : "false";
        
        // Generate sample logs
        string consoleLogs = "";
        if (sampleLogs != null && sampleLogs.Length > 0)
        {
            foreach (var log in sampleLogs)
            {
                consoleLogs += log;
            }
        }
        else
        {
            consoleLogs = @"<div class=""empty-console"">No logs available</div>";
        }
        
        string runtimeInfo = "";
        if (isNetworkActive)
        {
            string vpnText = isVpnInterface ? "Yes 🔒" : "No";
            string vpnClass = isVpnInterface ? "vpn-active" : "";
            
            runtimeInfo = $@"
                            <div class='panel-section runtime-info'>
                                <h3 class='section-title'>
                                    <svg class='section-icon' viewBox='0 0 256 256' fill='currentColor'>
                                        <path d='M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24Zm0,192a88,88,0,1,1,88-88A88.1,88.1,0,0,1,128,216Zm-8-80V80a8,8,0,0,1,16,0v56a8,8,0,0,1-16,0Zm20,36a12,12,0,1,1-12-12A12,12,0,0,1,140,172Z'/>
                                    </svg>
                                    Runtime Information
                                </h3>
                                <div class='info-grid'>
                                    <div class='info-item'>
                                        <span class='info-label'>Upstream Interface:</span>
                                        <span class='info-value'>{upstreamInterface ?? "Not detected"}</span>
                                    </div>
                                    <div class='info-item'>
                                        <span class='info-label'>VPN Interface:</span>
                                        <span class='info-value {vpnClass}'>{vpnText}</span>
                                    </div>
                                    <div class='info-item'>
                                        <span class='info-label'>Portal URL:</span>
                                        <span class='info-value url'>http://{gatewayIp}:{defaultPort}/portal</span>
                                    </div>
                                </div>
                            </div>";
        }
        
        string configLocked = "";
        if (isNetworkActive)
        {
            configLocked = @"
                            <div class='config-locked-notice'>
                                <svg viewBox='0 0 256 256' fill='currentColor'>
                                    <path d='M208,80H176V56a48,48,0,0,0-96,0V80H48A16,16,0,0,0,32,96V208a16,16,0,0,0,16,16H208a16,16,0,0,0,16-16V96A16,16,0,0,0,208,80ZM96,56a32,32,0,0,1,64,0V80H96ZM208,208H48V96H208V208Zm-80-40a12,12,0,1,1,12-12A12,12,0,0,1,128,168Z'/>
                                </svg>
                                <span>Configuration is locked while network is active. Stop the network to make changes.</span>
                            </div>";
        }
        
        return HTML_TEMPLATE
            .Replace("{LOGO_SRC}", finalLogoSrc)
            .Replace("{STATUS_CLASS}", statusClass)
            .Replace("{STATUS_TEXT}", statusText)
            .Replace("{START_DISABLED}", startDisabled)
            .Replace("{STOP_DISABLED}", stopDisabled)
            .Replace("{RESET_DISABLED}", resetDisabled)
            .Replace("{CONFIG_DISABLED}", configDisabled)
            .Replace("{RUNTIME_INFO}", runtimeInfo)
            .Replace("{CONFIG_LOCKED}", configLocked)
            .Replace("{INTERFACE}", wifiInterface)
            .Replace("{SSID}", ssid)
            .Replace("{PASSWORD}", password)
            .Replace("{PORT}", defaultPort.ToString())
            .Replace("{GATEWAY}", gatewayIp)
            .Replace("{DHCP_RANGE}", dhcpRange)
            .Replace("{CONSOLE_STATE}", consoleState)
            .Replace("{CONSOLE_EXPANDED}", consoleExpandedJs)
            .Replace("{CONSOLE_LOGS}", consoleLogs);
    }
    
    /// <summary>
    /// Generates a sample log entry for the console
    /// </summary>
    public static string GenerateLogEntry(string timestamp, string level, string message)
    {
        return $@"
                                <div class=""log-entry log-{level.ToLower()}"">
                                    <span class=""log-time"">[{timestamp}]</span>
                                    <span class=""log-level"">{level}</span>
                                    <span class=""log-message"">{message}</span>
                                </div>";
    }
    
    /// <summary>
    /// Generates sample logs for network startup sequence
    /// </summary>
    public static string[] GenerateNetworkStartupLogs()
    {
        return new[]
        {
            GenerateLogEntry("10:23:45", "INFO", "🚀 Network initialization started..."),
            GenerateLogEntry("10:23:46", "INFO", "📡 Configuring interface wlan0..."),
            GenerateLogEntry("10:23:47", "INFO", "🔧 Setting up access point 'EasyPeasy WiFi'..."),
            GenerateLogEntry("10:23:48", "WARNING", "⚠️ VPN interface not detected"),
            GenerateLogEntry("10:23:49", "INFO", "✓ Access Point started successfully on 192.168.4.1"),
            GenerateLogEntry("10:23:50", "INFO", "🌐 Portal available at http://192.168.4.1:80/portal")
        };
    }
}
