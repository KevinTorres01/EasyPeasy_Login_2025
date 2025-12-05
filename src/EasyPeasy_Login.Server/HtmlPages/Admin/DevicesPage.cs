namespace EasyPeasy_Login.Server.HtmlPages.Admin;

public static class DevicesPage
{
    private const string HTML_TEMPLATE = @"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Connected Devices Management - EasyPeasy Login</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Helvetica Neue', sans-serif;
            background: rgb(231,231,231);
            color: #000000;
            -webkit-font-smoothing: antialiased;
            -moz-osx-font-smoothing: grayscale;
            margin: 0;
            padding: 0;
        }

        .page {
            position: relative;
            display: flex;
            flex-direction: row;
            background-color: rgb(231,231,231);
        }

        /* Sidebar */
        .sidebar {
            width: 250px;
            height: 100vh;
            position: sticky;
            top: 0;
            background: #ffffff;
            border-right: 1px solid white;
            display: flex;
            flex-direction: column;
        }

        /* Bootstrap-like utilities */
        .ps-3 {
            padding-left: 1rem !important;
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

        .container-fluid {
            width: 100%;
            padding: 0;
        }

        .navbar-brand {
            font-size: 1.1rem;
            display: flex;
            align-items: center;
            padding-left: 1rem;
        }

        .brand-logo {
            height: 7rem;
            width: auto;
        }

        .nav-scrollable {
            background-color: white;
            border-right: 1px solid white;
            display: flex;
            flex-direction: column;
            height: calc(100vh - 7rem);
            overflow-y: auto;
        }

        .nav-scrollable nav {
            display: flex;
            flex-direction: column;
            height: 100%;
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
            background-color: rgb(231,231,231);
            flex: 1;
        }

        .main-content {
            padding-top: 5rem;
            padding-left: 5rem;
            padding-right: 5rem;
        }

        .top-header {
            display: flex;
            justify-content: flex-end;
            align-items: flex-end;
            padding: 0.5rem 0;
            font-size: 1rem;
            margin-bottom: 20px;
        }

        .top-header .page-tittle {
            margin: 0;
            text-align: right;
            font-size: 1rem;
            color: black;
        }

        .devices-page {
            min-height: 100vh;
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
            font-size: 24px;
            font-weight: 600;
            color: #000000;
            margin: 0 0 1rem 0;
        }

        .page-description {
            font-size: 14px;
            color: #6b6b6b;
            margin: 0;
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
        }

        /* Search and Filter */
        .search-filter-container {
            display: flex;
            gap: 1rem;
            margin-bottom: 1.5rem;
            align-items: center;
        }

        .search-box {
            flex: 1;
            max-width: 500px;
            position: relative;
            display: flex;
            align-items: center;
        }

        .search-box .search-icon {
            position: absolute;
            left: 14px;
            width: 18px;
            height: 18px;
            color: #6b6b6b;
            pointer-events: none;
        }

        .search-box input {
            width: 100%;
            padding: 12px 14px 12px 44px;
            border: 1px solid #e8e8e8;
            border-radius: 10px;
            font-size: 14px;
            background: white;
            color: #000000;
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
        }

        .search-box input::placeholder {
            color: #6b6b6b;
        }

        .search-box input:focus {
            outline: none;
            border-color: #5aabea;
            box-shadow: 0 0 0 3px rgba(90, 171, 234, 0.15);
        }

        .filter-dropdown select {
            padding: 12px 36px 12px 16px;
            border: 1px solid #e8e8e8;
            border-radius: 10px;
            font-size: 14px;
            background: white;
            color: #000000;
            cursor: pointer;
            appearance: none;
            background-image: url(""data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 256 256'%3E%3Cpath fill='%236b6b6b' d='M213.66,101.66l-80,80a8,8,0,0,1-11.32,0l-80-80A8,8,0,0,1,53.66,90.34L128,164.69l74.34-74.35a8,8,0,0,1,11.32,11.32Z'/%3E%3C/svg%3E"");
            background-repeat: no-repeat;
            background-position: right 12px center;
            min-width: 140px;
        }

        .filter-dropdown select:focus {
            outline: none;
            border-color: #5aabea;
        }

        /* Devices Table */
        .devices-table-container {
            background: white;
            border-radius: 12px;
            border: 1px solid #e8e8e8;
            overflow: hidden;
        }

        .devices-table {
            width: 100%;
            border-collapse: collapse;
        }

        .devices-table thead {
            background: #f9fafb;
        }

        .devices-table th {
            text-align: left;
            padding: 16px 24px;
            font-size: 14px;
            font-weight: 600;
            color: #000000;
            border-bottom: 1px solid #e8e8e8;
        }

        .devices-table td {
            padding: 16px 24px;
            font-size: 14px;
            color: #000000;
            border-bottom: 1px solid #e8e8e8;
        }

        .devices-table tbody tr:last-child td {
            border-bottom: none;
        }

        .devices-table tbody tr:hover {
            background: #f5f5f5;
        }

        .no-devices {
            text-align: center;
            color: #6b6b6b;
            padding: 40px !important;
            font-style: italic;
        }

        /* Actions Cell */
        .actions-cell {
            display: flex;
            gap: 8px;
            justify-content: flex-end;
        }

        .btn-action {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 36px;
            height: 36px;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.2s ease;
            opacity: 1 !important;
        }

        .btn-action svg {
            width: 16px;
            height: 16px;
            opacity: 1 !important;
        }

        .btn-disconnect {
            background: #c94a4a !important;
            color: white !important;
        }

        .btn-disconnect:hover {
            background: #b93a3a !important;
            transform: scale(1.05);
        }

        /* Modal Styles */
        .modal-overlay {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(0, 0, 0, 0.5);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 1000;
            animation: fadeIn 0.2s ease;
        }

        .modal-overlay.hidden {
            display: none;
        }

        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }

        .modal-content {
            background: white;
            border-radius: 16px;
            width: 100%;
            max-width: 480px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2);
            animation: slideUp 0.3s ease;
        }

        @keyframes slideUp {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 20px 24px;
            border-bottom: 1px solid #e8e8e8;
        }

        .modal-header h2 {
            margin: 0;
            font-size: 18px;
            font-weight: 600;
            color: #000000;
        }

        .btn-close-modal {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 32px;
            height: 32px;
            border: none;
            border-radius: 8px;
            background: transparent;
            color: #6b6b6b;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-close-modal:hover {
            background: #e8e8e8;
            color: #000000;
        }

        .btn-close-modal svg {
            width: 18px;
            height: 18px;
        }

        .modal-body {
            padding: 24px;
        }

        .modal-body p {
            margin: 0 0 12px 0;
            color: #000000;
        }

        .modal-delete .delete-warning {
            color: #c94a4a;
            font-size: 13px;
        }

        .modal-footer {
            display: flex;
            justify-content: flex-end;
            gap: 12px;
            padding: 16px 24px;
            border-top: 1px solid #e8e8e8;
            background: #f5f5f5;
            border-radius: 0 0 16px 16px;
        }

        .btn-cancel {
            padding: 10px 20px;
            border: 1px solid #e8e8e8;
            border-radius: 8px;
            background: white;
            color: #000000;
            font-size: 14px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-cancel:hover {
            background: #e8e8e8;
        }

        .btn-delete-confirm {
            padding: 10px 20px;
            border: none;
            border-radius: 8px;
            background: #c94a4a;
            color: white;
            font-size: 14px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-delete-confirm:hover {
            background: #b43a3a;
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
                padding-top: 2rem;
                padding-left: 2rem;
                padding-right: 2rem;
            }

            .search-filter-container {
                flex-direction: column;
                align-items: stretch;
            }

            .search-box {
                max-width: none;
            }

            .devices-table th,
            .devices-table td {
                padding: 12px 16px;
            }

            .actions-cell {
                flex-direction: column;
                gap: 4px;
            }

            .modal-content {
                margin: 16px;
                max-width: calc(100% - 32px);
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
                        <img src='{LOGO_SRC}' alt='EasyPeasy Logo' class='brand-logo'>
                    </div>
                </div>
            </div>
            <div class='nav-scrollable'>
                <nav class='nav flex-column'>
                    <div class='nav-item'>
                        <a href='#'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M104,40H56A16,16,0,0,0,40,56v48a16,16,0,0,0,16,16h48a16,16,0,0,0,16-16V56A16,16,0,0,0,104,40Zm0,64H56V56h48v48Zm96-64H152a16,16,0,0,0-16,16v48a16,16,0,0,0,16,16h48a16,16,0,0,0,16-16V56A16,16,0,0,0,200,40Zm0,64H152V56h48v48Zm-96,32H56a16,16,0,0,0-16,16v48a16,16,0,0,0,16,16h48a16,16,0,0,0,16-16V152A16,16,0,0,0,104,136Zm0,64H56V152h48v48Zm96-64H152a16,16,0,0,0-16,16v48a16,16,0,0,0,16,16h48a16,16,0,0,0,16-16V152A16,16,0,0,0,200,136Zm0,64H152V152h48v48Z'/>
                            </svg>
                            Dashboard
                        </a>
                    </div>
                    <div class='nav-item'>
                        <a href='#'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M117.25,157.92a60,60,0,1,0-66.5,0A95.83,95.83,0,0,0,3.53,195.63a8,8,0,1,0,13.4,8.74,80,80,0,0,1,134.14,0,8,8,0,0,0,13.4-8.74A95.83,95.83,0,0,0,117.25,157.92ZM40,108a44,44,0,1,1,44,44A44.05,44.05,0,0,1,40,108Zm210.14,98.7a8,8,0,0,1-11.07-2.33A79.83,79.83,0,0,0,172,168a8,8,0,0,1,0-16,44,44,0,1,0-16.34-84.87,8,8,0,1,1-5.94-14.85,60,60,0,0,1,55.53,105.64,95.83,95.83,0,0,1,47.22,37.71A8,8,0,0,1,250.14,206.7Z'/>
                            </svg>
                            User Accounts
                        </a>
                    </div>
                    <div class='nav-item'>
                        <a href='#' class='active'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M224,72H208V64a24,24,0,0,0-24-24H40A24,24,0,0,0,16,64v96a24,24,0,0,0,24,24H152v8a24,24,0,0,0,24,24h48a24,24,0,0,0,24-24V96A24,24,0,0,0,224,72ZM40,168a8,8,0,0,1-8-8V64a8,8,0,0,1,8-8H184a8,8,0,0,1,8,8v8H176a24,24,0,0,0-24,24v72Zm192,24a8,8,0,0,1-8,8H176a8,8,0,0,1-8-8V96a8,8,0,0,1,8-8h48a8,8,0,0,1,8,8Zm-96,16a8,8,0,0,1-8,8H88a8,8,0,0,1,0-16h40A8,8,0,0,1,136,208Zm80-96a8,8,0,0,1-8,8H192a8,8,0,0,1,0-16h16A8,8,0,0,1,216,112Z'/>
                            </svg>
                            Devices
                        </a>
                    </div>
                    <div class='nav-item'>
                        <a href='#'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M247.31,124.76c-.35-.79-8.82-19.58-27.65-38.41C194.57,61.26,162.88,48,128,48S61.43,61.26,36.34,86.35C17.51,105.18,9,124,8.69,124.76a8,8,0,0,0,0,6.5c.35.79,8.82,19.57,27.65,38.4C61.43,194.74,93.12,208,128,208s66.57-13.26,91.66-38.34c18.83-18.83,27.3-37.61,27.65-38.4A8,8,0,0,0,247.31,124.76ZM128,192c-30.78,0-57.67-11.19-79.93-33.25A133.47,133.47,0,0,1,25,128,133.33,133.33,0,0,1,48.07,97.25C70.33,75.19,97.22,64,128,64s57.67,11.19,79.93,33.25A133.46,133.46,0,0,1,231.05,128C223.84,141.46,192.43,192,128,192Zm0-112a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Z'/>
                                <circle cx='128' cy='128' r='24'/>
                            </svg>
                            Network Control
                        </a>
                    </div>
                    <div class='nav-item'>
                        <a href='#'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M128,80a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Zm88-29.84q.06-2.16,0-4.32l14.92-18.64a8,8,0,0,0,1.48-7.06,107.21,107.21,0,0,0-10.88-26.25,8,8,0,0,0-6-3.93l-23.72-2.64q-1.48-1.56-3-3L186,40.54a8,8,0,0,0-3.94-6,107.71,107.71,0,0,0-26.25-10.87,8,8,0,0,0-7.06,1.49L130.16,40Q128,40,125.84,40L107.2,25.11a8,8,0,0,0-7.06-1.48A107.6,107.6,0,0,0,73.89,34.51a8,8,0,0,0-3.93,6L67.32,64.27q-1.56,1.49-3,3L40.54,70a8,8,0,0,0-6,3.94,107.71,107.71,0,0,0-10.87,26.25,8,8,0,0,0,1.49,7.06L40,125.84Q40,128,40,130.16L25.11,148.8a8,8,0,0,0-1.48,7.06,107.21,107.21,0,0,0,10.88,26.25,8,8,0,0,0,6,3.93l23.72,2.64q1.49,1.56,3,3L70,215.46a8,8,0,0,0,3.94,6,107.71,107.71,0,0,0,26.25,10.87,8,8,0,0,0,7.06-1.49L125.84,216q2.16.06,4.32,0l18.64,14.92a8,8,0,0,0,7.06,1.48,107.21,107.21,0,0,0,26.25-10.88,8,8,0,0,0,3.93-6l2.64-23.72q1.56-1.48,3-3L215.46,186a8,8,0,0,0,6-3.94,107.71,107.71,0,0,0,10.87-26.25,8,8,0,0,0-1.49-7.06Zm-16.1-6.5a73.93,73.93,0,0,1,0,8.68,8,8,0,0,0,1.74,5.48l14.19,17.73a91.57,91.57,0,0,1-6.23,15L187,173.11a8,8,0,0,0-5.1,2.64,74.11,74.11,0,0,1-6.14,6.14,8,8,0,0,0-2.64,5.1l-2.51,22.58a91.32,91.32,0,0,1-15,6.23l-17.74-14.19a8,8,0,0,0-5-1.75h-.48a73.93,73.93,0,0,1-8.68,0,8,8,0,0,0-5.48,1.74L100.45,215.8a91.57,91.57,0,0,1-15-6.23L82.89,187a8,8,0,0,0-2.64-5.1,74.11,74.11,0,0,1-6.14-6.14,8,8,0,0,0-5.1-2.64L46.43,170.6a91.32,91.32,0,0,1-6.23-15l14.19-17.74a8,8,0,0,0,1.74-5.48,73.93,73.93,0,0,1,0-8.68,8,8,0,0,0-1.74-5.48L40.2,100.45a91.57,91.57,0,0,1,6.23-15L69,82.89a8,8,0,0,0,5.1-2.64,74.11,74.11,0,0,1,6.14-6.14A8,8,0,0,0,82.89,69L85.4,46.43a91.32,91.32,0,0,1,15-6.23l17.74,14.19a8,8,0,0,0,5.48,1.74,73.93,73.93,0,0,1,8.68,0,8,8,0,0,0,5.48-1.74L155.55,40.2a91.57,91.57,0,0,1,15,6.23L173.11,69a8,8,0,0,0,2.64,5.1,74.11,74.11,0,0,1,6.14,6.14,8,8,0,0,0,5.1,2.64l22.58,2.51a91.32,91.32,0,0,1,6.23,15l-14.19,17.74A8,8,0,0,0,199.87,123.66Z'/>
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
                <div class='top-header'>
                    <h1 class='page-tittle'>Administrator System</h1>
                </div>

                <div class='devices-page'>
                    <!-- Header -->
                    <div class='page-header-wrapper'>
                        <div class='page-header'>
                            <div class='header-text'>
                                <h1 class='page-title'>Connected Devices Management</h1>
                                <p class='page-description'>Session Management</p>
                            </div>
                            <button class='btn-add-user' onclick='refreshDevices()'>
                                <svg class='btn-icon' viewBox='0 0 256 256' fill='currentColor'>
                                    <path d='M197.67,186.37a8,8,0,0,1,0,11.29C196.58,198.73,170.82,224,128,224c-37.39,0-64.53-22.4-80-39.85V208a8,8,0,0,1-16,0V160a8,8,0,0,1,8-8H88a8,8,0,0,1,0,16H55.44C67.76,183.35,93,208,128,208c36,0,58.14-21.46,58.36-21.68A8,8,0,0,1,197.67,186.37ZM216,40a8,8,0,0,0-8,8V71.85C192.53,54.4,165.39,32,128,32,85.18,32,59.42,57.27,58.34,58.34a8,8,0,0,0,11.3,11.34C69.86,69.46,92,48,128,48c35,0,60.24,24.65,72.56,40H168a8,8,0,0,0,0,16h48a8,8,0,0,0,8-8V48A8,8,0,0,0,216,40Z'/>
                                </svg>
                                Refresh
                            </button>
                        </div>
                    </div>

                    <!-- Search and Filter -->
                    <div class='search-filter-container'>
                        <div class='search-box'>
                            <svg class='search-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M229.66,218.34l-50.07-50.06a88.11,88.11,0,1,0-11.31,11.31l50.06,50.07a8,8,0,0,0,11.32-11.32ZM40,112a72,72,0,1,1,72,72A72.08,72.08,0,0,1,40,112Z'/>
                            </svg>
                            <input type='text' id='searchInput' placeholder='Search by username, IP, MAC...' oninput='filterDevices()' />
                        </div>
                        <div class='filter-dropdown'>
                            <select id='statusFilter' onchange='filterDevices()'>
                                <option value='all'>Status: All</option>
                            </select>
                        </div>
                    </div>

                    <!-- Devices Table -->
                    <div class='devices-table-container'>
                        <table class='devices-table'>
                            <thead>
                                <tr>
                                    <th>IP Address</th>
                                    <th>MAC Address</th>
                                    <th>User Name</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody id='devicesTableBody'>
                                {DEVICE_ROWS}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </main>
    </div>

    <!-- Disconnect Confirmation Modal -->
    <div class='modal-overlay hidden' id='disconnectModal' onclick='closeDisconnectModal()'>
        <div class='modal-content modal-delete' onclick='event.stopPropagation()'>
            <div class='modal-header'>
                <h2>Confirm Disconnect</h2>
            </div>
            <div class='modal-body'>
                <p>Are you sure you want to disconnect device <strong id='disconnectMacAddress'></strong>?</p>
                <p class='delete-warning'>The user will be logged out immediately.</p>
            </div>
            <div class='modal-footer'>
                <button class='btn-cancel' onclick='closeDisconnectModal()'>Cancel</button>
                <button class='btn-delete-confirm' onclick='confirmDisconnect()'>Disconnect</button>
            </div>
        </div>
    </div>

    <script>
        // Device data store
        let devices = {DEVICES_DATA};
        let disconnectingMac = '';

        // Filter devices
        function filterDevices() {{
            const searchTerm = document.getElementById('searchInput').value.toLowerCase();
            
            const filtered = devices.filter(device => {{
                return device.username.toLowerCase().includes(searchTerm) || 
                       device.ipAddress.toLowerCase().includes(searchTerm) ||
                       device.macAddress.toLowerCase().includes(searchTerm);
            }});

            renderDevices(filtered);
        }}

        // Render devices in table
        function renderDevices(filteredDevices) {{
            const tbody = document.getElementById('devicesTableBody');
            
            if (filteredDevices.length === 0) {{
                tbody.innerHTML = '<tr><td colspan=""4"" class=""no-devices"">No connected devices found</td></tr>';
                return;
            }}

            tbody.innerHTML = filteredDevices.map(device => `
                <tr>
                    <td>${{device.ipAddress}}</td>
                    <td>${{device.macAddress}}</td>
                    <td>${{device.username}}</td>
                    <td class=""actions-cell"">
                        <button class=""btn-action btn-disconnect"" title=""Disconnect device"" onclick=""openDisconnectModal('${{device.macAddress}}')"">
                            <svg viewBox=""0 0 256 256"" fill=""currentColor"">
                                <path d=""M216,48H176V40a24,24,0,0,0-24-24H104A24,24,0,0,0,80,40v8H40a8,8,0,0,0,0,16h8V208a16,16,0,0,0,16,16H192a16,16,0,0,0,16-16V64h8a8,8,0,0,0,0-16ZM96,40a8,8,0,0,1,8-8h48a8,8,0,0,1,8,8v8H96Zm96,168H64V64H192ZM112,104v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Zm48,0v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Z""/>
                            </svg>
                        </button>
                    </td>
                </tr>
            `).join('');
        }}

        // Refresh devices
        function refreshDevices() {{
            // In a real app, this would fetch from server
            filterDevices();
        }}

        // Open disconnect modal
        function openDisconnectModal(macAddress) {{
            disconnectingMac = macAddress;
            document.getElementById('disconnectMacAddress').textContent = macAddress;
            document.getElementById('disconnectModal').classList.remove('hidden');
        }}

        // Close disconnect modal
        function closeDisconnectModal() {{
            document.getElementById('disconnectModal').classList.add('hidden');
            disconnectingMac = '';
        }}

        // Confirm disconnect
        function confirmDisconnect() {{
            devices = devices.filter(d => d.macAddress !== disconnectingMac);
            closeDisconnectModal();
            filterDevices();
        }}

        // Initialize
        renderDevices(devices);
    </script>
</body>
</html>";

    /// <summary>
    /// Generates the devices management page
    /// </summary>
    public static string GenerateDevices(
        List<(string ipAddress, string macAddress, string username)>? devices = null,
        string? logoSrc = null)
    {
        string finalLogoSrc = logoSrc ?? DashboardPageConst.LOGO_DATA_URI;

        // Default sample devices if none provided
        var sampleDevices = devices ?? new List<(string ipAddress, string macAddress, string username)>
        {
            ("192.168.4.10", "AA:BB:CC:DD:EE:01", "admin"),
            ("192.168.4.15", "AA:BB:CC:DD:EE:02", "user1"),
            ("192.168.4.20", "AA:BB:CC:DD:EE:03", "sarah.w"),
            ("192.168.4.25", "AA:BB:CC:DD:EE:04", "mike.j"),
            ("192.168.4.30", "AA:BB:CC:DD:EE:05", "robert.b")
        };

        // Generate JavaScript array
        var devicesJson = "[" + string.Join(",", sampleDevices.Select(d => 
            $"{{ipAddress:\"{d.ipAddress}\",macAddress:\"{d.macAddress}\",username:\"{d.username}\"}}")) + "]";

        // Generate table rows for no-JS fallback
        string deviceRows = "";
        if (sampleDevices.Count == 0)
        {
            deviceRows = "<tr><td colspan='4' class='no-devices'>No connected devices found</td></tr>";
        }
        else
        {
            foreach (var device in sampleDevices)
            {
                deviceRows += $@"
                            <tr>
                                <td>{device.ipAddress}</td>
                                <td>{device.macAddress}</td>
                                <td>{device.username}</td>
                                <td class='actions-cell'>
                                    <button class='btn-action btn-disconnect' title='Disconnect device' onclick='openDisconnectModal(""{device.macAddress}"")'>
                                        <svg viewBox='0 0 256 256' fill='currentColor'>
                                            <path d='M216,48H176V40a24,24,0,0,0-24-24H104A24,24,0,0,0,80,40v8H40a8,8,0,0,0,0,16h8V208a16,16,0,0,0,16,16H192a16,16,0,0,0,16-16V64h8a8,8,0,0,0,0-16ZM96,40a8,8,0,0,1,8-8h48a8,8,0,0,1,8,8v8H96Zm96,168H64V64H192ZM112,104v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Zm48,0v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Z'/>
                                        </svg>
                                    </button>
                                </td>
                            </tr>";
            }
        }

        return HTML_TEMPLATE
            .Replace("{LOGO_SRC}", finalLogoSrc)
            .Replace("{DEVICES_DATA}", devicesJson)
            .Replace("{DEVICE_ROWS}", deviceRows);
    }
}
