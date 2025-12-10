using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _apiBaseUrl;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _apiBaseUrl = _configuration["ApiBaseUrl"] ?? "http://localhost:5226";
    }

    public async Task<bool> SendWelcomeEmailAsync(string email, string employeeName)
    {
        try
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var smtpUser = _configuration["Smtp:User"];
            var smtpPassword = _configuration["Smtp:Password"];

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
                return false;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TalentoPlus - RRHH", smtpUser));
            message.To.Add(new MailboxAddress(employeeName, email));
            message.Subject = "🎉 ¡Bienvenido a TalentoPlus! - Tu cuenta ha sido creada";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = GetWelcomeEmailHtml(employeeName, email),
                TextBody = GetWelcomeEmailText(employeeName)
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GetWelcomeEmailHtml(string employeeName, string email)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; background-color: #f0f4f8;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f0f4f8; padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 32px; font-weight: 700;'>🚀 TalentoPlus</h1>
                            <p style='color: rgba(255,255,255,0.9); margin: 10px 0 0 0; font-size: 16px;'>Sistema de Gestión de Recursos Humanos</p>
                        </td>
                    </tr>
                    
                    <!-- Welcome Banner -->
                    <tr>
                        <td style='padding: 40px 30px 20px 30px; text-align: center;'>
                            <div style='font-size: 60px; margin-bottom: 15px;'>🎉</div>
                            <h2 style='color: #1e293b; margin: 0; font-size: 28px; font-weight: 600;'>¡Bienvenido/a, {employeeName}!</h2>
                            <p style='color: #64748b; font-size: 16px; margin-top: 10px;'>Tu cuenta ha sido creada exitosamente</p>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 20px 30px;'>
                            <p style='color: #475569; font-size: 15px; line-height: 1.7; margin: 0 0 20px 0;'>
                                Es un placer darte la bienvenida a nuestro equipo. Ahora tienes acceso a tu portal de empleado donde podrás gestionar tu información personal y laboral.
                            </p>
                            
                            <!-- Features Grid -->
                            <table width='100%' cellpadding='0' cellspacing='0' style='margin: 25px 0;'>
                                <tr>
                                    <td width='50%' style='padding: 10px;'>
                                        <div style='background-color: #f1f5f9; border-radius: 12px; padding: 20px; text-align: center;'>
                                            <div style='font-size: 30px; margin-bottom: 10px;'>📋</div>
                                            <h3 style='color: #334155; margin: 0 0 5px 0; font-size: 14px; font-weight: 600;'>Mi Información</h3>
                                            <p style='color: #64748b; margin: 0; font-size: 12px;'>Consulta tus datos personales y laborales</p>
                                        </div>
                                    </td>
                                    <td width='50%' style='padding: 10px;'>
                                        <div style='background-color: #f1f5f9; border-radius: 12px; padding: 20px; text-align: center;'>
                                            <div style='font-size: 30px; margin-bottom: 10px;'>📄</div>
                                            <h3 style='color: #334155; margin: 0 0 5px 0; font-size: 14px; font-weight: 600;'>Hoja de Vida PDF</h3>
                                            <p style='color: #64748b; margin: 0; font-size: 12px;'>Descarga tu curriculum actualizado</p>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td width='50%' style='padding: 10px;'>
                                        <div style='background-color: #f1f5f9; border-radius: 12px; padding: 20px; text-align: center;'>
                                            <div style='font-size: 30px; margin-bottom: 10px;'>✏️</div>
                                            <h3 style='color: #334155; margin: 0 0 5px 0; font-size: 14px; font-weight: 600;'>Actualizar Datos</h3>
                                            <p style='color: #64748b; margin: 0; font-size: 12px;'>Mantén tu información al día</p>
                                        </div>
                                    </td>
                                    <td width='50%' style='padding: 10px;'>
                                        <div style='background-color: #f1f5f9; border-radius: 12px; padding: 20px; text-align: center;'>
                                            <div style='font-size: 30px; margin-bottom: 10px;'>🔐</div>
                                            <h3 style='color: #334155; margin: 0 0 5px 0; font-size: 14px; font-weight: 600;'>Acceso Seguro</h3>
                                            <p style='color: #64748b; margin: 0; font-size: 12px;'>Autenticación JWT protegida</p>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- CTA Buttons -->
                    <tr>
                        <td style='padding: 10px 30px 30px 30px; text-align: center;'>
                            <p style='color: #64748b; font-size: 14px; margin: 0 0 20px 0;'>Accede a la documentación de la API para comenzar:</p>
                            <a href='{_apiBaseUrl}/swagger' style='display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; padding: 14px 35px; border-radius: 8px; font-weight: 600; font-size: 15px; margin: 5px;'>
                                📖 Ver API (Swagger)
                            </a>
                        </td>
                    </tr>
                    
                    <!-- API Info Box -->
                    <tr>
                        <td style='padding: 0 30px 30px 30px;'>
                            <div style='background-color: #eff6ff; border-left: 4px solid #3b82f6; border-radius: 8px; padding: 20px;'>
                                <h4 style='color: #1e40af; margin: 0 0 10px 0; font-size: 14px;'>📡 Endpoints disponibles para ti:</h4>
                                <table width='100%' cellpadding='5' cellspacing='0' style='font-size: 13px;'>
                                    <tr>
                                        <td style='color: #059669; font-weight: 600; width: 70px;'>POST</td>
                                        <td style='color: #475569;'>/api/auth/login - Iniciar sesión</td>
                                    </tr>
                                    <tr>
                                        <td style='color: #3b82f6; font-weight: 600;'>GET</td>
                                        <td style='color: #475569;'>/api/empleado/me - Ver mi información</td>
                                    </tr>
                                    <tr>
                                        <td style='color: #3b82f6; font-weight: 600;'>GET</td>
                                        <td style='color: #475569;'>/api/empleado/me/pdf - Descargar mi hoja de vida</td>
                                    </tr>
                                    <tr>
                                        <td style='color: #f59e0b; font-weight: 600;'>PUT</td>
                                        <td style='color: #475569;'>/api/empleado/me/contact - Actualizar contacto</td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Account Info -->
                    <tr>
                        <td style='padding: 0 30px 30px 30px;'>
                            <div style='background-color: #fefce8; border: 1px solid #fef08a; border-radius: 8px; padding: 15px;'>
                                <p style='color: #854d0e; margin: 0; font-size: 13px;'>
                                    <strong>📧 Tu correo de acceso:</strong> {email}<br>
                                    <strong>🔑 Contraseña:</strong> La que configuraste al registrarte
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8fafc; padding: 25px 30px; text-align: center; border-top: 1px solid #e2e8f0;'>
                            <p style='color: #64748b; font-size: 13px; margin: 0 0 10px 0;'>
                                ¿Tienes preguntas? Contacta al departamento de <strong>Recursos Humanos</strong>
                            </p>
                            <p style='color: #94a3b8; font-size: 11px; margin: 0;'>
                                © 2025 TalentoPlus - Sistema de Gestión de RRHH<br>
                                Este es un correo automático, por favor no responda a este mensaje.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private static string GetWelcomeEmailText(string employeeName)
    {
        return $@"
¡Bienvenido a TalentoPlus!

Estimado/a {employeeName},

Es un placer darte la bienvenida a nuestro equipo. Tu registro en el sistema TalentoPlus ha sido completado exitosamente.

A través de este sistema podrás:
- Consultar tu información personal y laboral
- Descargar tu hoja de vida en formato PDF
- Mantener actualizada tu información de contacto

Si tienes alguna pregunta, no dudes en contactar al departamento de Recursos Humanos.

¡Éxitos en esta nueva etapa!

El equipo de TalentoPlus

---
Este es un correo automático, por favor no responda a este mensaje.
";
    }
}
