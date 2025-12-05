namespace EasyPeasy_Login.Server.HtmlPages.Admin;

public static class UserManagementPage
{
    private const string HTML_TEMPLATE = @"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>User Account Management - EasyPeasy Login</title>
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
        }

        .top-header .page-tittle {
            margin: 0;
            text-align: right;
            font-size: 1rem;
            color: black;
        }

        .users-page {
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

        /* Users Table */
        .users-table-container {
            background: white;
            border-radius: 12px;
            border: 1px solid #e8e8e8;
            overflow: hidden;
        }

        .users-table {
            width: 100%;
            border-collapse: collapse;
        }

        .users-table thead {
            background: #f9fafb;
        }

        .users-table th {
            text-align: left;
            padding: 16px 24px;
            font-size: 14px;
            font-weight: 600;
            color: #000000;
            border-bottom: 1px solid #e8e8e8;
        }

        .users-table td {
            padding: 16px 24px;
            font-size: 14px;
            color: #000000;
            border-bottom: 1px solid #e8e8e8;
        }

        .users-table tbody tr:last-child td {
            border-bottom: none;
        }

        .users-table tbody tr:hover {
            background: #f5f5f5;
        }

        /* Status Badge */
        .status-badge {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            border-radius: 20px;
            font-size: 13px;
            font-weight: 500;
        }

        .status-badge.status-active {
            background: #a5e0e0;
            color: #000000;
        }

        .status-badge.status-inactive {
            background: #e8e8e8;
            color: #6b6b6b;
        }

        .status-badge .status-dot {
            width: 8px;
            height: 8px;
            border-radius: 50%;
        }

        .status-badge.status-active .status-dot {
            background: #2a9d9d;
        }

        .status-badge.status-inactive .status-dot {
            background: #6b6b6b;
        }

        /* Action Buttons */
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

        .btn-edit {
            background: #5aabea !important;
            color: white !important;
        }

        .btn-edit:hover {
            background: #4a9bda !important;
            transform: scale(1.05);
        }

        .btn-delete {
            background: #c94a4a !important;
            color: white !important;
        }

        .btn-delete:hover {
            background: #b93a3a !important;
            transform: scale(1.05);
        }

        .btn-toggle {
            background: #e85a8f !important;
            color: white !important;
        }

        .btn-toggle:hover {
            background: #d84a7f !important;
            transform: scale(1.05);
        }

        .no-users {
            text-align: center;
            color: #6b6b6b;
            padding: 40px !important;
            font-style: italic;
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

        .modal-body .form-group {
            margin-bottom: 20px;
        }

        .modal-body .form-group:last-child {
            margin-bottom: 0;
        }

        .modal-body .form-group label {
            display: block;
            margin-bottom: 8px;
            font-size: 14px;
            font-weight: 500;
            color: #000000;
        }

        .modal-body .form-group input,
        .modal-body .form-group select {
            width: 100%;
            padding: 12px 14px;
            border: 1px solid #e8e8e8;
            border-radius: 8px;
            font-size: 14px;
            color: #000000;
            transition: border-color 0.2s ease;
        }

        .modal-body .form-group input:focus,
        .modal-body .form-group select:focus {
            outline: none;
            border-color: #5aabea;
        }

        .modal-body .form-group input:disabled {
            background: #e8e8e8;
            cursor: not-allowed;
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

        .btn-save {
            padding: 10px 20px;
            border: none;
            border-radius: 8px;
            background: #5aabea;
            color: white;
            font-size: 14px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .btn-save:hover {
            background: #4a9bda;
        }

        /* Delete Modal */
        .modal-delete .modal-body p {
            margin: 0 0 12px 0;
            color: #000000;
        }

        .modal-delete .delete-warning {
            color: #c94a4a;
            font-size: 13px;
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

            .users-table th,
            .users-table td {
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
                        <a href='#' class='active'>
                            <svg class='nav-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M117.25,157.92a60,60,0,1,0-66.5,0A95.83,95.83,0,0,0,3.53,195.63a8,8,0,1,0,13.4,8.74,80,80,0,0,1,134.14,0,8,8,0,0,0,13.4-8.74A95.83,95.83,0,0,0,117.25,157.92ZM40,108a44,44,0,1,1,44,44A44.05,44.05,0,0,1,40,108Zm210.14,98.7a8,8,0,0,1-11.07-2.33A79.83,79.83,0,0,0,172,168a8,8,0,0,1,0-16,44,44,0,1,0-16.34-84.87,8,8,0,1,1-5.94-14.85,60,60,0,0,1,55.53,105.64,95.83,95.83,0,0,1,47.22,37.71A8,8,0,0,1,250.14,206.7Z'/>
                            </svg>
                            User Accounts
                        </a>
                    </div>
                    <div class='nav-item'>
                        <a href='#'>
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

                <div class='users-page'>
                <!-- Header -->
                <div class='page-header-wrapper'>
                    <div class='page-header'>
                        <div class='header-text'>
                            <h1 class='page-title'>User Account Management</h1>
                            <p class='page-description'>Portal users management</p>
                        </div>
                        <button class='btn-add-user' onclick='openAddUserModal()'>
                            <svg class='btn-icon' viewBox='0 0 256 256' fill='currentColor'>
                                <path d='M228,128a12,12,0,0,1-12,12H140v76a12,12,0,0,1-24,0V140H40a12,12,0,0,1,0-24h76V40a12,12,0,0,1,24,0v76h76A12,12,0,0,1,228,128Z' />
                            </svg>
                            Add User
                        </button>
                    </div>
                </div>

                <!-- Search and Filter -->
                <div class='search-filter-container'>
                    <div class='search-box'>
                        <svg class='search-icon' viewBox='0 0 256 256' fill='currentColor'>
                            <path d='M229.66,218.34l-50.07-50.06a88.11,88.11,0,1,0-11.31,11.31l50.06,50.07a8,8,0,0,0,11.32-11.32ZM40,112a72,72,0,1,1,72,72A72.08,72.08,0,0,1,40,112Z'/>
                        </svg>
                        <input type='text' id='searchInput' placeholder='Search by username, name, ...' oninput='filterUsers()' />
                    </div>
                    <div class='filter-dropdown'>
                        <select id='statusFilter' onchange='filterUsers()'>
                            <option value='all'>Status: All</option>
                            <option value='active'>Status: Active</option>
                            <option value='inactive'>Status: Inactive</option>
                        </select>
                    </div>
                </div>

                <!-- Users Table -->
                <div class='users-table-container'>
                    <table class='users-table'>
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>User Name</th>
                                <th>User Status</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody id='usersTableBody'>
                            {USER_ROWS}
                        </tbody>
                    </table>
                </div>
            </div>
        </main>
    </div>

    <!-- Add/Edit User Modal -->
    <div class='modal-overlay hidden' id='userModal' onclick='closeUserModal()'>
        <div class='modal-content' onclick='event.stopPropagation()'>
            <div class='modal-header'>
                <h2 id='modalTitle'>Add New User</h2>
                <button class='btn-close-modal' onclick='closeUserModal()'>
                    <svg viewBox='0 0 256 256' fill='currentColor'>
                        <path d='M205.66,194.34a8,8,0,0,1-11.32,11.32L128,139.31,61.66,205.66a8,8,0,0,1-11.32-11.32L116.69,128,50.34,61.66A8,8,0,0,1,61.66,50.34L128,116.69l66.34-66.35a8,8,0,0,1,11.32,11.32L139.31,128Z'/>
                    </svg>
                </button>
            </div>
            <div class='modal-body'>
                <div class='form-group'>
                    <label>Name</label>
                    <input type='text' id='inputName' placeholder='Enter full name' />
                </div>
                <div class='form-group'>
                    <label>Username</label>
                    <input type='text' id='inputUsername' placeholder='Enter username' />
                </div>
                <div class='form-group' id='passwordGroup'>
                    <label>Password</label>
                    <input type='password' id='inputPassword' placeholder='Enter password' />
                </div>
                <div class='form-group'>
                    <label>Status</label>
                    <select id='inputStatus'>
                        <option value='true'>Active</option>
                        <option value='false'>Inactive</option>
                    </select>
                </div>
            </div>
            <div class='modal-footer'>
                <button class='btn-cancel' onclick='closeUserModal()'>Cancel</button>
                <button class='btn-save' onclick='saveUser()' id='btnSave'>Add User</button>
            </div>
        </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div class='modal-overlay hidden' id='deleteModal' onclick='closeDeleteModal()'>
        <div class='modal-content modal-delete' onclick='event.stopPropagation()'>
            <div class='modal-header'>
                <h2>Confirm Delete</h2>
            </div>
            <div class='modal-body'>
                <p>Are you sure you want to delete user <strong id='deleteUsername'></strong>?</p>
                <p class='delete-warning'>This action cannot be undone.</p>
            </div>
            <div class='modal-footer'>
                <button class='btn-cancel' onclick='closeDeleteModal()'>Cancel</button>
                <button class='btn-delete-confirm' onclick='confirmDelete()'>Delete</button>
            </div>
        </div>
    </div>

    <script>
        // User data store
        let users = {USERS_DATA};
        let isEditing = false;
        let editingUsername = '';
        let deletingUsername = '';

        // Filter users
        function filterUsers() {{
            const searchTerm = document.getElementById('searchInput').value.toLowerCase();
            const statusFilter = document.getElementById('statusFilter').value;
            
            const filtered = users.filter(user => {{
                const matchesSearch = user.name.toLowerCase().includes(searchTerm) || 
                                    user.username.toLowerCase().includes(searchTerm);
                const matchesStatus = statusFilter === 'all' || 
                                     (statusFilter === 'active' && user.isActive) || 
                                     (statusFilter === 'inactive' && !user.isActive);
                return matchesSearch && matchesStatus;
            }});

            renderUsers(filtered);
        }}

        // Render users in table
        function renderUsers(filteredUsers) {{
            const tbody = document.getElementById('usersTableBody');
            
            if (filteredUsers.length === 0) {{
                tbody.innerHTML = '<tr><td colspan=""4"" class=""no-users"">No users found</td></tr>';
                return;
            }}

            tbody.innerHTML = filteredUsers.map(user => `
                <tr>
                    <td>${{user.name}}</td>
                    <td>${{user.username}}</td>
                    <td>
                        <span class=""status-badge ${{user.isActive ? 'status-active' : 'status-inactive'}}"">
                            <span class=""status-dot""></span>
                            ${{user.isActive ? 'Active' : 'Inactive'}}
                        </span>
                    </td>
                    <td class=""actions-cell"">
                        <button class=""btn-action btn-edit"" title=""Edit user"" onclick=""openEditUserModal('${{user.username}}')"">
                            <svg viewBox=""0 0 256 256"" fill=""currentColor"">
                                <path d=""M227.31,73.37,182.63,28.68a16,16,0,0,0-22.63,0L36.69,152A15.86,15.86,0,0,0,32,163.31V208a16,16,0,0,0,16,16H92.69A15.86,15.86,0,0,0,104,219.31L227.31,96a16,16,0,0,0,0-22.63ZM92.69,208H48V163.31l88-88L180.69,120ZM192,108.68,147.31,64l24-24L216,84.68Z""/>
                            </svg>
                        </button>
                        <button class=""btn-action btn-delete"" title=""Delete user"" onclick=""openDeleteModal('${{user.username}}')"">
                            <svg viewBox=""0 0 256 256"" fill=""currentColor"">
                                <path d=""M216,48H176V40a24,24,0,0,0-24-24H104A24,24,0,0,0,80,40v8H40a8,8,0,0,0,0,16h8V208a16,16,0,0,0,16,16H192a16,16,0,0,0,16-16V64h8a8,8,0,0,0,0-16ZM96,40a8,8,0,0,1,8-8h48a8,8,0,0,1,8,8v8H96Zm96,168H64V64H192ZM112,104v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Zm48,0v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Z""/>
                            </svg>
                        </button>
                        <button class=""btn-action btn-toggle"" title=""${{user.isActive ? 'Deactivate user' : 'Activate user'}}"" onclick=""toggleUserStatus('${{user.username}}')"">
                            <svg viewBox=""0 0 256 256"" fill=""currentColor"">
                                <path d=""M247.31,124.76c-.35-.79-8.82-19.58-27.65-38.41C194.57,61.26,162.88,48,128,48S61.43,61.26,36.34,86.35C17.51,105.18,9,124,8.69,124.76a8,8,0,0,0,0,6.5c.35.79,8.82,19.57,27.65,38.4C61.43,194.74,93.12,208,128,208s66.57-13.26,91.66-38.34c18.83-18.83,27.3-37.61,27.65-38.4A8,8,0,0,0,247.31,124.76ZM128,192c-30.78,0-57.67-11.19-79.93-33.25A133.47,133.47,0,0,1,25,128,133.33,133.33,0,0,1,48.07,97.25C70.33,75.19,97.22,64,128,64s57.67,11.19,79.93,33.25A133.46,133.46,0,0,1,231.05,128C223.84,141.46,192.43,192,128,192Zm0-112a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Z""/>
                            </svg>
                        </button>
                    </td>
                </tr>
            `).join('');
        }}

        // Open add user modal
        function openAddUserModal() {{
            isEditing = false;
            document.getElementById('modalTitle').textContent = 'Add New User';
            document.getElementById('btnSave').textContent = 'Add User';
            document.getElementById('inputName').value = '';
            document.getElementById('inputUsername').value = '';
            document.getElementById('inputPassword').value = '';
            document.getElementById('inputStatus').value = 'true';
            document.getElementById('inputUsername').disabled = false;
            document.getElementById('passwordGroup').style.display = 'block';
            document.getElementById('userModal').classList.remove('hidden');
        }}

        // Open edit user modal
        function openEditUserModal(username) {{
            isEditing = true;
            editingUsername = username;
            const user = users.find(u => u.username === username);
            if (!user) return;

            document.getElementById('modalTitle').textContent = 'Edit User';
            document.getElementById('btnSave').textContent = 'Save Changes';
            document.getElementById('inputName').value = user.name;
            document.getElementById('inputUsername').value = user.username;
            document.getElementById('inputPassword').value = '';
            document.getElementById('inputStatus').value = user.isActive.toString();
            document.getElementById('inputUsername').disabled = true;
            document.getElementById('passwordGroup').style.display = 'none';
            document.getElementById('userModal').classList.remove('hidden');
        }}

        // Close user modal
        function closeUserModal() {{
            document.getElementById('userModal').classList.add('hidden');
            editingUsername = '';
        }}

        // Save user
        function saveUser() {{
            const name = document.getElementById('inputName').value.trim();
            const username = document.getElementById('inputUsername').value.trim();
            const password = document.getElementById('inputPassword').value;
            const isActive = document.getElementById('inputStatus').value === 'true';

            if (!name || !username) {{
                alert('Please fill in all required fields');
                return;
            }}

            if (!isEditing && !password) {{
                alert('Password is required for new users');
                return;
            }}

            if (isEditing) {{
                // Update existing user
                const user = users.find(u => u.username === editingUsername);
                if (user) {{
                    user.name = name;
                    user.isActive = isActive;
                }}
            }} else {{
                // Add new user
                if (users.some(u => u.username === username)) {{
                    alert('Username already exists');
                    return;
                }}
                users.push({{ name, username, isActive }});
            }}

            closeUserModal();
            filterUsers();
        }}

        // Open delete modal
        function openDeleteModal(username) {{
            deletingUsername = username;
            document.getElementById('deleteUsername').textContent = username;
            document.getElementById('deleteModal').classList.remove('hidden');
        }}

        // Close delete modal
        function closeDeleteModal() {{
            document.getElementById('deleteModal').classList.add('hidden');
            deletingUsername = '';
        }}

        // Confirm delete
        function confirmDelete() {{
            users = users.filter(u => u.username !== deletingUsername);
            closeDeleteModal();
            filterUsers();
        }}

        // Toggle user status
        function toggleUserStatus(username) {{
            const user = users.find(u => u.username === username);
            if (user) {{
                user.isActive = !user.isActive;
                filterUsers();
            }}
        }}

        // Initialize
        renderUsers(users);
    </script>
</body>
</html>";

    /// <summary>
    /// Generates the user management page
    /// </summary>
    public static string GenerateUserManagement(
        List<(string name, string username, bool isActive)>? users = null,
        string? logoSrc = null)
    {
        string finalLogoSrc = logoSrc ?? DashboardPageConst.LOGO_DATA_URI;

        // Default sample users if none provided
        var sampleUsers = users ?? new List<(string name, string username, bool isActive)>
        {
            ("John Doe", "admin", true),
            ("Jane Smith", "user1", true),
            ("Mike Johnson", "mike.j", false),
            ("Sarah Williams", "sarah.w", true),
            ("Robert Brown", "robert.b", true)
        };

        // Generate JavaScript array
        var usersJson = "[" + string.Join(",", sampleUsers.Select(u => 
            $"{{name:\"{u.name}\",username:\"{u.username}\",isActive:{u.isActive.ToString().ToLower()}}}")) + "]";

        // Generate table rows for no-JS fallback
        string userRows = "";
        if (sampleUsers.Count == 0)
        {
            userRows = "<tr><td colspan='4' class='no-users'>No users found</td></tr>";
        }
        else
        {
            foreach (var user in sampleUsers)
            {
                string statusClass = user.isActive ? "status-active" : "status-inactive";
                string statusText = user.isActive ? "Active" : "Inactive";
                string toggleTitle = user.isActive ? "Deactivate user" : "Activate user";

                userRows += $@"
                            <tr>
                                <td>{user.name}</td>
                                <td>{user.username}</td>
                                <td>
                                    <span class='status-badge {statusClass}'>
                                        <span class='status-dot'></span>
                                        {statusText}
                                    </span>
                                </td>
                                <td class='actions-cell'>
                                    <button class='btn-action btn-edit' title='Edit user' onclick='openEditUserModal(""{user.username}"")'>
                                        <svg viewBox='0 0 256 256' fill='currentColor'>
                                            <path d='M227.31,73.37,182.63,28.68a16,16,0,0,0-22.63,0L36.69,152A15.86,15.86,0,0,0,32,163.31V208a16,16,0,0,0,16,16H92.69A15.86,15.86,0,0,0,104,219.31L227.31,96a16,16,0,0,0,0-22.63ZM92.69,208H48V163.31l88-88L180.69,120ZM192,108.68,147.31,64l24-24L216,84.68Z'/>
                                        </svg>
                                    </button>
                                    <button class='btn-action btn-delete' title='Delete user' onclick='openDeleteModal(""{user.username}"")'>
                                        <svg viewBox='0 0 256 256' fill='currentColor'>
                                            <path d='M216,48H176V40a24,24,0,0,0-24-24H104A24,24,0,0,0,80,40v8H40a8,8,0,0,0,0,16h8V208a16,16,0,0,0,16,16H192a16,16,0,0,0,16-16V64h8a8,8,0,0,0,0-16ZM96,40a8,8,0,0,1,8-8h48a8,8,0,0,1,8,8v8H96Zm96,168H64V64H192ZM112,104v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Zm48,0v64a8,8,0,0,1-16,0V104a8,8,0,0,1,16,0Z'/>
                                        </svg>
                                    </button>
                                    <button class='btn-action btn-toggle' title='{toggleTitle}' onclick='toggleUserStatus(""{user.username}"")'>
                                        <svg viewBox='0 0 256 256' fill='currentColor'>
                                            <path d='M247.31,124.76c-.35-.79-8.82-19.58-27.65-38.41C194.57,61.26,162.88,48,128,48S61.43,61.26,36.34,86.35C17.51,105.18,9,124,8.69,124.76a8,8,0,0,0,0,6.5c.35.79,8.82,19.57,27.65,38.4C61.43,194.74,93.12,208,128,208s66.57-13.26,91.66-38.34c18.83-18.83,27.3-37.61,27.65-38.4A8,8,0,0,0,247.31,124.76ZM128,192c-30.78,0-57.67-11.19-79.93-33.25A133.47,133.47,0,0,1,25,128,133.33,133.33,0,0,1,48.07,97.25C70.33,75.19,97.22,64,128,64s57.67,11.19,79.93,33.25A133.46,133.46,0,0,1,231.05,128C223.84,141.46,192.43,192,128,192Zm0-112a48,48,0,1,0,48,48A48.05,48.05,0,0,0,128,80Zm0,80a32,32,0,1,1,32-32A32,32,0,0,1,128,160Z'/>
                                        </svg>
                                    </button>
                                </td>
                            </tr>";
            }
        }

        return HTML_TEMPLATE
            .Replace("{LOGO_SRC}", finalLogoSrc)
            .Replace("{USERS_DATA}", usersJson)
            .Replace("{USER_ROWS}", userRows);
    }
}
