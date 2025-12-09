using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
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
            message.Subject = "¡Bienvenido a TalentoPlus!";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = GetWelcomeEmailHtml(employeeName),
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

    private static string GetWelcomeEmailHtml(string employeeName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2563eb; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f8fafc; padding: 30px; border-radius: 0 0 8px 8px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #64748b; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Bienvenido a TalentoPlus!</h1>
        </div>
        <div class='content'>
            <p>Estimado/a <strong>{employeeName}</strong>,</p>
            <p>Es un placer darte la bienvenida a nuestro equipo. Tu registro en el sistema TalentoPlus ha sido completado exitosamente.</p>
            <p>A través de este sistema podrás:</p>
            <ul>
                <li>Consultar tu información personal y laboral</li>
                <li>Descargar tu hoja de vida en formato PDF</li>
                <li>Mantener actualizada tu información de contacto</li>
            </ul>
            <p>Si tienes alguna pregunta, no dudes en contactar al departamento de Recursos Humanos.</p>
            <p>¡Éxitos en esta nueva etapa!</p>
            <p><strong>El equipo de TalentoPlus</strong></p>
        </div>
        <div class='footer'>
            <p>Este es un correo automático, por favor no responda a este mensaje.</p>
        </div>
    </div>
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
