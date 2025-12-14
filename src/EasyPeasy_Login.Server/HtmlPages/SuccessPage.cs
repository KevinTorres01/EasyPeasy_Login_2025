using System.Text;

namespace EasyPeasy_Login.Server.HtmlPages
{
    /// <summary>
    /// Success page turned of Blazor to pure HTML for HTTP Server with sockets
    /// </summary>
    public static class SuccessPage
    {
        /// <summary>
        /// Template HTML of success page with place holders
        /// </summary>
        private const string HTML_TEMPLATE = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Success!</title>
    <style>
        /* Reset global */
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        html, body {
            height: 100%;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            -webkit-font-smoothing: antialiased;
            -moz-osx-font-smoothing: grayscale;
        }

        body {
            margin: 0;
            padding: 0;
        }

        /* Success Page */
        .success-page {
            min-height: 100vh;
            background: #6b7280;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .success-card {
            background: rgb(231, 231, 231);
            border-radius: 20px;
            padding: 80px 60px;
            text-align: center;
            width: 100%;
            max-width: 1100px;
            min-height: 600px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            animation: fadeIn 0.5s ease-out;
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .success-title {
            font-size: 42px;
            font-weight: 700;
            color: #000;
            margin-bottom: 8px;
            animation: slideIn 0.6s ease-out 0.2s both;
        }

        .success-subtitle {
            font-size: 18px;
            color: #000;
            margin-bottom: 24px;
            animation: slideIn 0.6s ease-out 0.4s both;
        }

        .terms-link {
            font-size: 14px;
            color: #6b7280;
            animation: slideIn 0.6s ease-out 0.6s both;
        }

        .terms-link a {
            color: #000;
            text-decoration: underline;
            font-weight: 500;
            transition: opacity 0.2s;
        }

        .terms-link a:hover {
            opacity: 0.7;
        }

        /* Logout Button */
        .logout-container {
            margin-top: 40px;
            animation: slideIn 0.6s ease-out 0.8s both;
        }

        .btn-logout {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            padding: 14px 28px;
            background: #000000;
            color: white;
            border: none;
            border-radius: 10px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            text-decoration: none;
        }

        .btn-logout:hover {
            background: #4b5563;
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(107, 114, 128, 0.4);
        }

        .btn-logout:active {
            transform: translateY(0);
        }

        .btn-logout svg {
            width: 20px;
            height: 20px;
            flex-shrink: 0;
        }

        .btn-logout:disabled {
            opacity: 0.6;
            cursor: not-allowed;
            transform: none;
        }

        .logout-message {
            margin-top: 12px;
            font-size: 14px;
            color: #6b7280;
            min-height: 20px;
        }

        .logout-message.error {
            color: #dc2626;
        }

        .logout-message.success {
            color: #059669;
        }

        @keyframes slideIn {
            from {
                opacity: 0;
                transform: translateY(10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        /* Confetti animation */
        .confetti {
            position: fixed;
            width: 10px;
            height: 10px;
            background: #f0f;
            position: absolute;
            animation: confetti-fall 3s linear infinite;
        }

        @keyframes confetti-fall {
            to {
                transform: translateY(100vh) rotate(360deg);
                opacity: 0;
            }
        }

        /* Responsive */
        @media (max-width: 768px) {
            .success-card {
                padding: 60px 30px;
                min-height: 300px;
            }

            .success-title {
                font-size: 28px;
            }

            .success-subtitle {
                font-size: 16px;
            }
        }
    </style>
</head>
<body>
    <div class=""success-page"">
        <div class=""success-card"">
            <h1 class=""success-title"">{{TITLE}}</h1>
            <p class=""success-subtitle"">{{SUBTITLE}}</p>
            <p class=""terms-link"">
                {{TERMS_LINK_TEXT}} <a href=""{{TERMS_LINK_URL}}"">{{TERMS_LINK_ANCHOR}}</a>?
            </p>
            
            <!-- Logout Button -->
            <div class=""logout-container"">
                <button class=""btn-logout"" id=""logoutBtn"" onclick=""handleLogout()"">
                    <svg viewBox=""0 0 256 256"" fill=""currentColor"">
                        <path d=""M120,216a8,8,0,0,1-8,8H48a8,8,0,0,1-8-8V40a8,8,0,0,1,8-8h64a8,8,0,0,1,0,16H56V208h56A8,8,0,0,1,120,216Zm109.66-93.66-40-40a8,8,0,0,0-11.32,11.32L204.69,120H112a8,8,0,0,0,0,16h92.69l-26.35,26.34a8,8,0,0,0,11.32,11.32l40-40A8,8,0,0,0,229.66,122.34Z""/>
                    </svg>
                    {{LOGOUT_BUTTON_TEXT}}
                </button>
                <p class=""logout-message"" id=""logoutMessage""></p>
            </div>
        </div>
    </div>

    <script>
        // Create confetti effect
        function createConfetti() {
            const colors = ['#bb313e', '#f6a8c9', '#e84e7e', '#3b95f5', '#80d8d0'];
            const confettiCount = 50;
            
            for (let i = 0; i < confettiCount; i++) {
                setTimeout(() => {
                    const confetti = document.createElement('div');
                    confetti.className = 'confetti';
                    confetti.style.left = Math.random() * 100 + '%';
                    confetti.style.top = -10 + 'px';
                    confetti.style.background = colors[Math.floor(Math.random() * colors.length)];
                    confetti.style.animationDelay = Math.random() * 3 + 's';
                    confetti.style.animationDuration = (Math.random() * 2 + 2) + 's';
                    document.body.appendChild(confetti);
                    
                    // Remove after animation
                    setTimeout(() => confetti.remove(), 5000);
                }, i * 50);
            }
        }

        // Logout handler
        async function handleLogout() {
            const btn = document.getElementById('logoutBtn');
            const msg = document.getElementById('logoutMessage');
            
            btn.disabled = true;
            btn.innerHTML = `
                <svg viewBox=""0 0 256 256"" fill=""currentColor"" style=""animation: spin 1s linear infinite;"">
                    <path d=""M232,128a104,104,0,0,1-208,0c0-41,23.81-78.36,60.66-95.27a8,8,0,0,1,6.68,14.54C60.15,61.59,40,93.27,40,128a88,88,0,0,0,176,0c0-34.73-20.15-66.41-51.34-80.73a8,8,0,0,1,6.68-14.54C208.19,49.64,232,87,232,128Z""/>
                </svg>
                {{LOGOUT_LOADING_TEXT}}
            `;
            msg.textContent = '';
            msg.className = 'logout-message';
            
            try {
                const response = await fetch('/api/logout', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                });
                
                const data = await response.json();
                
                if (response.ok && data.success) {
                    msg.textContent = '{{LOGOUT_SUCCESS_MESSAGE}}';
                    msg.className = 'logout-message success';
                    btn.innerHTML = `
                        <svg viewBox=""0 0 256 256"" fill=""currentColor"">
                            <path d=""M229.66,77.66l-128,128a8,8,0,0,1-11.32,0l-56-56a8,8,0,0,1,11.32-11.32L96,188.69,218.34,66.34a8,8,0,0,1,11.32,11.32Z""/>
                        </svg>
                        {{LOGOUT_DONE_TEXT}}
                    `;
                    // Redirect to portal after 2 seconds
                    setTimeout(() => {
                        window.location.href = '/portal';
                    }, 2000);
                } else {
                    throw new Error(data.error || data.message || 'Logout failed');
                }
            } catch (error) {
                msg.textContent = error.message || '{{LOGOUT_ERROR_MESSAGE}}';
                msg.className = 'logout-message error';
                btn.disabled = false;
                btn.innerHTML = `
                    <svg viewBox=""0 0 256 256"" fill=""currentColor"">
                        <path d=""M120,216a8,8,0,0,1-8,8H48a8,8,0,0,1-8-8V40a8,8,0,0,1,8-8h64a8,8,0,0,1,0,16H56V208h56A8,8,0,0,1,120,216Zm109.66-93.66-40-40a8,8,0,0,0-11.32,11.32L204.69,120H112a8,8,0,0,0,0,16h92.69l-26.35,26.34a8,8,0,0,0,11.32,11.32l40-40A8,8,0,0,0,229.66,122.34Z""/>
                    </svg>
                    {{LOGOUT_BUTTON_TEXT}}
                `;
            }
        }

        // Trigger confetti on load
        window.addEventListener('load', () => {
            setTimeout(createConfetti, 300);
        });
    </script>
    <style>
        @keyframes spin {
            from { transform: rotate(0deg); }
            to { transform: rotate(360deg); }
        }
    </style>
</body>
</html>";

        /// <summary>
        /// Generate success page with specified values
        /// </summary>
        public static string GenerateSuccessPage(
            string? title = null,
            string? subtitle = null,
            string? termsLinkText = null,
            string? termsLinkAnchor = null,
            string? termsLinkUrl = null,
            string? logoutButtonText = null,
            string? logoutLoadingText = null,
            string? logoutDoneText = null,
            string? logoutSuccessMessage = null,
            string? logoutErrorMessage = null)
        {
            var html = HTML_TEMPLATE;
            
            // Replace placeholders with default values
            html = html.Replace("{{TITLE}}", EscapeHtml(title ?? "Welcome to the internet!"));
            html = html.Replace("{{SUBTITLE}}", EscapeHtml(subtitle ?? "EasyPeasy, right?"));
            html = html.Replace("{{TERMS_LINK_TEXT}}", EscapeHtml(termsLinkText ?? "Have you seen the"));
            html = html.Replace("{{TERMS_LINK_ANCHOR}}", EscapeHtml(termsLinkAnchor ?? "terms and conditions"));
            html = html.Replace("{{TERMS_LINK_URL}}", EscapeHtml(termsLinkUrl ?? "/portal/terms"));
            
            // Logout button placeholders
            html = html.Replace("{{LOGOUT_BUTTON_TEXT}}", EscapeHtml(logoutButtonText ?? "Disconnect"));
            html = html.Replace("{{LOGOUT_LOADING_TEXT}}", EscapeHtml(logoutLoadingText ?? "Disconnecting..."));
            html = html.Replace("{{LOGOUT_DONE_TEXT}}", EscapeHtml(logoutDoneText ?? "Disconnected"));
            html = html.Replace("{{LOGOUT_SUCCESS_MESSAGE}}", EscapeHtml(logoutSuccessMessage ?? "You have been disconnected. Redirecting to portal..."));
            html = html.Replace("{{LOGOUT_ERROR_MESSAGE}}", EscapeHtml(logoutErrorMessage ?? "Failed to disconnect. Please try again."));
            
            return html;
        }

        /// <summary>
        /// Generate success page in Spanish
        /// </summary>
        public static string GenerateSuccessPageSpanish()
        {
            return GenerateSuccessPage(
                title: "¡Bienvenido a Internet!",
                subtitle: "Fácil, ¿verdad?",
                termsLinkText: "¿Has visto los",
                termsLinkAnchor: "términos y condiciones",
                termsLinkUrl: "/portal/terms",
                logoutButtonText: "Desconectar",
                logoutLoadingText: "Desconectando...",
                logoutDoneText: "Desconectado",
                logoutSuccessMessage: "Te has desconectado. Redirigiendo al portal...",
                logoutErrorMessage: "Error al desconectar. Inténtalo de nuevo."
            );
        }

        /// <summary>
        /// Generate success page in English
        /// </summary>
        public static string GenerateSuccessPageEnglish()
        {
            return GenerateSuccessPage(
                title: "Welcome to the internet!",
                subtitle: "EasyPeasy, right?",
                termsLinkText: "Have you seen the",
                termsLinkAnchor: "terms and conditions",
                termsLinkUrl: "/portal/terms",
                logoutButtonText: "Disconnect",
                logoutLoadingText: "Disconnecting...",
                logoutDoneText: "Disconnected",
                logoutSuccessMessage: "You have been disconnected. Redirecting to portal...",
                logoutErrorMessage: "Failed to disconnect. Please try again."
            );
        }

        /// <summary>
        /// Generate a customized success page
        /// </summary>
        public static string GenerateCustomSuccessPage(
            string title,
            string subtitle,
            string? termsLinkUrl = null,
            string? logoutButtonText = null)
        {
            return GenerateSuccessPage(
                title: title,
                subtitle: subtitle,
                termsLinkText: "Have you seen the",
                termsLinkAnchor: "terms and conditions",
                termsLinkUrl: termsLinkUrl ?? "/portal/terms",
                logoutButtonText: logoutButtonText
            );
        }

        /// <summary>
        /// Scape character HTML to prevent XSS
        /// </summary>
        private static string EscapeHtml(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }
    }
}
