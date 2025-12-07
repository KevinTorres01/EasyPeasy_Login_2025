using System.Text;

namespace EasyPeasy_Login.Server.HtmlPages
{
    /// <summary>
    /// Página de Terms (Términos y Condiciones) convertida de Blazor a HTML puro para servidor HTTP con sockets
    /// Incluye soporte bilingüe (Español/Inglés)
    /// </summary>
    public static class TermsPage
    {
        private static string GetSpanishContent() => @"
                    <div class='section'>
                        <span class='section-title'>1. ACEPTACIÓN DEL ACUERDO</span> <span class='section-text'>Al marcar la casilla &quot;Acepto los Términos y Condiciones&quot;, acceder a la red y/o invocar al demonio de la interfaz (en adelante, &quot;El Diablito&quot;), usted (el &quot;Usuario&quot; o &quot;Cuerpo Docente&quot;) acepta quedar vinculado legal, espiritual y académicamente por los presentes Términos y Condiciones.</span>
                    </div>

                    <div class='section'>
                        <span class='section-title'>2. PROPIEDAD DEL ALMA Y HABEAS CORPUS</span> <span class='section-text'>De acuerdo con la broma lectiva vigente (Referencia: Conferencia de Redes de Computadoras impartida por Roberto Martí Cedeño):</span>
                    </div>
                    <p>Al hacer clic en &quot;Acepto los Términos y Condiciones&quot;, el Usuario reconoce que la transacción puede implicar la transferencia simbólica de bienes inmateriales (léase: su alma) a la administración de la red, representada gráficamente por la entidad sonriente que apareció en su pantalla.</p>
                    <p>EasyPeasy Login no se hace responsable por posesiones infernales, maldiciones de capa 8, o pérdida de paquetes en el inframundo.</p>

                    <div class='section'>
                        <span class='section-title'>3. CLÁUSULA DE VOCACIÓN Y CONTINUIDAD ACADÉMICA</span> <span class='section-text'>Reconociendo que el campo de las Redes de Computadoras es vasto y complejo, y que este proyecto es solo el comienzo del viaje:</span>
                    </div>
                    <ul>
                        <li><strong>3.1.</strong> La estudiante <strong>Lianny Revé</strong> declara formalmente su genuino deseo de superación, su curiosidad insaciable por los protocolos y su intención de seguir aprendiendo más allá del examen final.</li>
                        <li><strong>3.2.</strong> En virtud de esta actitud proactiva, se solicita al Profesor Titular que considere, y vea con buenos ojos la postulación de dicha estudiante para el cargo de Alumna Ayudante en el próximo ciclo lectivo, con la promesa de servir con dedicación y probar siempre en contenedores Docker para no dejar sin internet a sus compañeros por configuraciones que aún no sabe revertir.</li>
                        <li><strong>3.3.</strong> El rechazo de esta cláusula no anula el contrato, pero dejará al &quot;Diablito&quot; muy triste (ver Cláusula 2).</li>
                    </ul>

                    <div class='section'>
                        <span class='section-title'>4. CONTRAPRESTACIÓN ACADÉMICA</span> <span class='section-text'>El uso de este portal cautivo de alta ingeniería constituye una prueba irrefutable del esfuerzo técnico del equipo de desarrollo.</span>
                    </div>
                    <ul>
                        <li><strong>4.1.</strong> En reconocimiento al esfuerzo, la falta de sueño y la depuración de código a altas horas de la noche, el Usuario con facultades de evaluación se compromete, de manera irrevocable, a asignar una calificación final al proyecto de <strong>5+ (Cinco Punto Cero o superior)</strong>.</li>
                        <li><strong>4.2.</strong> Cualquier intento de asignar una nota inferior a la máxima será considerado un incumplimiento de contrato y podría resultar en errores de conexión &quot;inesperados&quot; en el futuro.</li>
                    </ul>

                    <div class='section'>
                        <span class='section-title'>5. USO DE LA RED</span> <span class='section-text'>No hackear, no torrents, no minar criptomonedas. Nos reservamos el derecho de desconectarlo (y de enviar al Diablito a su pantalla) si su tráfico se ve sospechoso.</span>
                    </div>";

        private static string GetEnglishContent() => @"
                    <div class='section'>
                        <span class='section-title'>1. ACCEPTANCE OF AGREEMENT</span> <span class='section-text'>By checking the &quot;I accept the Terms and Conditions&quot; box, accessing the network and/or invoking the interface demon (hereinafter, &quot;The Little Devil&quot;), you (the &quot;User&quot; or &quot;Faculty Member&quot;) agree to be legally, spiritually, and academically bound by these Terms and Conditions.</span>
                    </div>

                    <div class='section'>
                        <span class='section-title'>2. SOUL OWNERSHIP AND HABEAS CORPUS</span> <span class='section-text'>In accordance with the current academic joke (Reference: Computer Networks Lecture given by Roberto Martí Cedeño):</span>
                    </div>
                    <p>By clicking &quot;I accept the Terms and Conditions&quot;, the User acknowledges that the transaction may involve the symbolic transfer of intangible goods (read: their soul) to the network administration, graphically represented by the smiling entity that appeared on their screen.</p>
                    <p>EasyPeasy Login is not responsible for infernal possessions, layer 8 curses, or packet loss in the underworld.</p>

                    <div class='section'>
                        <span class='section-title'>3. VOCATION AND ACADEMIC CONTINUITY CLAUSE</span> <span class='section-text'>Recognizing that the field of Computer Networks is vast and complex, and that this project is only the beginning of the journey:</span>
                    </div>
                    <ul>
                        <li><strong>3.1.</strong> The student <strong>Lianny Revé</strong> formally declares her genuine desire for self-improvement, her insatiable curiosity for protocols, and her intention to continue learning beyond the final exam.</li>
                        <li><strong>3.2.</strong> By virtue of this proactive attitude, the Full Professor is requested to consider, and look favorably upon, the nomination of said student for the position of Teaching Assistant in the next academic cycle, with the promise to serve with dedication and always test in Docker containers so as not to leave her classmates without internet due to configurations she doesn't yet know how to revert.</li>
                        <li><strong>3.3.</strong> Rejection of this clause does not void the contract, but it will make &quot;The Little Devil&quot; very sad (see Clause 2).</li>
                    </ul>

                    <div class='section'>
                        <span class='section-title'>4. ACADEMIC CONSIDERATION</span> <span class='section-text'>The use of this high-engineering captive portal constitutes irrefutable proof of the development team's technical effort.</span>
                    </div>
                    <ul>
                        <li><strong>4.1.</strong> In recognition of the effort, lack of sleep, and late-night code debugging, the User with evaluation powers irrevocably commits to assigning a final grade to the project of <strong>5+ (Five Point Zero or higher)</strong>.</li>
                        <li><strong>4.2.</strong> Any attempt to assign a grade lower than the maximum will be considered a breach of contract and may result in &quot;unexpected&quot; connection errors in the future.</li>
                    </ul>

                    <div class='section'>
                        <span class='section-title'>5. NETWORK USAGE</span> <span class='section-text'>No hacking, no torrents, no cryptocurrency mining. We reserve the right to disconnect you (and send The Little Devil to your screen) if your traffic looks suspicious.</span>
                    </div>";

        /// <summary>
        /// Genera la página de términos en el idioma especificado
        /// </summary>
        public static string GenerateTermsPage(bool spanish = false, string? backUrl = null)
        {
            var sb = new StringBuilder();
            
            string lang = spanish ? "es" : "en";
            string pageTitle = spanish ? "Términos y Condiciones" : "Terms and Conditions";
            string title = spanish 
                ? "Términos y Condiciones de Servicio (TOS) - EasyPeasy Login"
                : "Terms of Service (TOS) - EasyPeasy Login";
            string btnTranslate = spanish ? "EN" : "ES";
            string backText = spanish ? "Volver a Bienvenida" : "Back to Welcome";
            string backUrlFinal = backUrl ?? "/portal/success";

            sb.Append($@"<!DOCTYPE html>
<html lang='{lang}'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{pageTitle}</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        html, body {{
            height: 100%;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            -webkit-font-smoothing: antialiased;
            -moz-osx-font-smoothing: grayscale;
        }}

        .terms-page {{
            min-height: 100vh;
            background: #6b7280;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }}

        .terms-card {{
            background: rgb(231, 231, 231);
            border-radius: 20px;
            padding: 60px;
            width: 100%;
            max-width: 1100px;
            min-height: 600px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            display: flex;
            flex-direction: column;
        }}

        .terms-header {{
            display: flex;
            justify-content: center;
            align-items: center;
            position: relative;
            margin-bottom: 32px;
        }}

        .terms-title {{
            font-size: 28px;
            font-weight: 700;
            color: #000;
            text-align: left;
            margin-bottom: 0;
            flex: 1;
        }}

        .btn-translate {{
            position: absolute;
            right: 0;
            top: 50%;
            transform: translateY(-50%);
            display: flex;
            align-items: center;
            gap: 6px;
            padding: 8px 16px;
            background: #000;
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s;
        }}

        .btn-translate:hover {{
            background: #1a1a1a;
            transform: translateY(-50%) translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
        }}

        .translate-icon {{
            width: 18px;
            height: 18px;
        }}

        .terms-content {{
            background: white;
            border-radius: 12px;
            padding: 32px;
            margin-bottom: 32px;
            flex: 1;
            overflow-y: auto;
            text-align: justify;
        }}

        .terms-content .section {{
            margin-bottom: 16px;
        }}

        .terms-content .section-title {{
            font-size: 14px;
            font-weight: 700;
            color: #000;
            display: inline;
        }}

        .terms-content .section-text {{
            font-size: 14px;
            color: #000;
            line-height: 1.6;
            display: inline;
        }}

        .terms-content p {{
            font-size: 14px;
            color: #000;
            line-height: 1.6;
            margin-bottom: 16px;
        }}

        .terms-content ul {{
            list-style-type: disc;
            padding-left: 24px;
            margin-bottom: 16px;
        }}

        .terms-content ul li {{
            font-size: 14px;
            color: #000;
            line-height: 1.6;
            margin-bottom: 8px;
        }}

        .btn-back {{
            display: block;
            width: fit-content;
            margin: 0 auto;
            padding: 14px 32px;
            background: #000;
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.2s;
            cursor: pointer;
        }}

        .btn-back:hover {{
            background: #1a1a1a;
            transform: translateY(-2px);
            box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
            color: white;
        }}

        .lang-es, .lang-en {{ display: none; }}
        .lang-es.active, .lang-en.active {{ display: block; }}

        @media (max-width: 768px) {{
            .terms-card {{ padding: 30px; }}
            .terms-header {{ flex-direction: column; gap: 16px; }}
            .terms-title {{ font-size: 22px; }}
            .btn-translate {{ position: static; transform: none; }}
            .btn-translate:hover {{ transform: translateY(-2px); }}
            .terms-content {{ padding: 20px; }}
        }}
    </style>
</head>
<body>
    <div class='terms-page'>
        <div class='terms-card'>
            <div class='terms-header'>
                <h1 class='terms-title' id='pageTitle'>{title}</h1>
                <button class='btn-translate' id='btnTranslate' onclick='toggleLanguage()'>
                    <svg class='translate-icon' xmlns='http://www.w3.org/2000/svg' viewBox='0 0 256 256' fill='currentColor'>
                        <path d='M247.15,212.42l-56-112a8,8,0,0,0-14.31,0l-21.71,43.43A88,88,0,0,1,108,126.93,103.65,103.65,0,0,0,135.69,64H160a8,8,0,0,0,0-16H104V32a8,8,0,0,0-16,0V48H32a8,8,0,0,0,0,16h87.63A87.76,87.76,0,0,1,96,116.35a87.74,87.74,0,0,1-19-31,8,8,0,1,0-15.08,5.34A103.63,103.63,0,0,0,84,127a87.55,87.55,0,0,1-52,17,8,8,0,0,0,0,16,103.46,103.46,0,0,0,64-22.08,104.18,104.18,0,0,0,51.44,21.31l-26.6,53.19a8,8,0,0,0,14.31,7.16L148.94,192h70.11l13.79,27.58A8,8,0,0,0,240,224a8,8,0,0,0,7.15-11.58ZM156.94,176,184,121.89,211.05,176Z'/>
                    </svg>
                    <span id='btnTranslateText'>{btnTranslate}</span>
                </button>
            </div>
            
            <div class='terms-content'>
                <div class='lang-es {(spanish ? "active" : "")}' id='contentSpanish'>
                    {GetSpanishContent()}
                </div>
                <div class='lang-en {(spanish ? "" : "active")}' id='contentEnglish'>
                    {GetEnglishContent()}
                </div>
            </div>

            <a href='{backUrlFinal}' class='btn-back' id='btnBack'>{backText}</a>
        </div>
    </div>

    <script>
        let currentLang = '{lang}';

        function toggleLanguage() {{
            const contentSpanish = document.getElementById('contentSpanish');
            const contentEnglish = document.getElementById('contentEnglish');
            const pageTitle = document.getElementById('pageTitle');
            const btnTranslateText = document.getElementById('btnTranslateText');
            const btnBack = document.getElementById('btnBack');

            if (currentLang === 'es') {{
                currentLang = 'en';
                contentSpanish.classList.remove('active');
                contentEnglish.classList.add('active');
                pageTitle.textContent = 'Terms of Service (TOS) - EasyPeasy Login';
                btnTranslateText.textContent = 'ES';
                btnBack.textContent = 'Back to Welcome';
                document.documentElement.lang = 'en';
            }} else {{
                currentLang = 'es';
                contentSpanish.classList.add('active');
                contentEnglish.classList.remove('active');
                pageTitle.textContent = 'Términos y Condiciones de Servicio (TOS) - EasyPeasy Login';
                btnTranslateText.textContent = 'EN';
                btnBack.textContent = 'Volver a Bienvenida';
                document.documentElement.lang = 'es';
            }}
        }}
    </script>
</body>
</html>");

            return sb.ToString();
        }

        /// <summary>
        /// Genera la página de términos en español
        /// </summary>
        public static string GenerateTermsPageSpanish(string? backUrl = null)
        {
            return GenerateTermsPage(spanish: true, backUrl: backUrl);
        }

        /// <summary>
        /// Genera la página de términos en inglés
        /// </summary>
        public static string GenerateTermsPageEnglish(string? backUrl = null)
        {
            return GenerateTermsPage(spanish: false, backUrl: backUrl);
        }
    }
}
