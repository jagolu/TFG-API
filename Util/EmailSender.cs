using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace API.Util
{
    public static class EmailSender
    {
        private static SmtpClient _client;
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration config) {
            _configuration = config;
            initializeClient();
        }

        public static void sendVerificationToken(string email, string name, string tokenValidation)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(_configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = getBodySendTokenEmailVerification(name, tokenValidation);
            msg.IsBodyHtml = true;
            msg.Subject = "Email confirmation";

            _client.Send(msg);
        }

        public static void sendVerificationPasswordToken(string email, string name, string passwordToken)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(_configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = getBodySendTokenChangePassword(name, email, passwordToken);
            msg.IsBodyHtml = true;
            msg.Subject = "Reset your password";

            _client.Send(msg);
        }

        public static void sendBanNotification(string email, string name, bool ban)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(_configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = ban ? getBodySendBanNotification(name) : getBodySendUnBanNotification(name);
            msg.IsBodyHtml = true;
            msg.Subject = ban ? "!Has sido baneado de VirtualBet!" : "Tu cuenta de VirtualBet ha sido desbloqueada!";

            _client.Send(msg);
        }

        public static void sendDMNotification(string email, string name, string dmTitle)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(_configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = getBodySendDMNotification(name, dmTitle);
            msg.IsBodyHtml = true;
            msg.Subject = "Un administrador ha respondido en tu conversación";

            _client.Send(msg);
        }

        private static void initializeClient()
        {
            _client = new SmtpClient("smtp.gmail.com");
            _client.UseDefaultCredentials = false;
            _client.Port = 587;
            string email = _configuration["Email:username"];
            string pass = _configuration["Email:password"];
            _client.Credentials = new NetworkCredential(email, pass);
            _client.EnableSsl = true;
        }

        private static String getBodySendTokenEmailVerification(string name, string tokenVerfication)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hello "+name+"</h1><br>";
            body += "<p>Click on the link below to confirm your email: </p>";
            body += _configuration["URL"]+"/emailVerification/" + tokenVerfication;
            body += "</body></html>";
            return body;
        }

        private static String getBodySendTokenChangePassword(string name, string email, string tokenPassword)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hello "+name+"</h1><br>";
            body += "<p>You have ask for a new password for VirtualBet on this email ("+email+"): </p>";
            body += "<p>If you really forgot the password click on the following link: </p>";
            body += _configuration["URL"]+ "/changePassword/" + tokenPassword;
            body += "<br><p>If you did't ask for this don't follow the link. </p>";
            body += "</body></html>";
            return body;
        }

        private static String getBodySendBanNotification(string name)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Los administradores de la plataforma te han baneado</p>";
            body += "<p>No podrás volver a iniciar sesión hasta que te la desbloqueen.</p>";
            body += "<p>No hay tiempo exacto para que desbloqueen la cuenta.</p>";
            body += "<p>Si crees que esto es un error escribe un correo electrónico a la direccion "+ _configuration["EmailIssues:username"] + "@gmail.com</p>";
            body += "</body></html>";
            return body;
        }

        private static String getBodySendUnBanNotification(string name)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Los administradores de la plataforma han desbloqueado tu cuenta</p>";
            body += "<p>Puedes volver a acceder a todas sus funcionalidades con normalidad.</p>";
            body += "</body></html>";
            return body;
        }

        private static String getBodySendDMNotification(string name, string dmTitle)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Uno de los administradores de la plataforma te ha contestado en la ";
            body += "conversación sobre el tema " + dmTitle + "</p>";
            body += "<p>Si tienes otro problema no dudes en abrir otra conversación.</p>";
            body += "</body></html>";
            return body;
        }
    }
}
