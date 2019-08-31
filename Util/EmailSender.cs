using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace API.Util
{
    public static class EmailSender
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The smtp client to send the email</value>
        private static SmtpClient _client;
        
        /// <value>The configuration of the application</value>
        private static IConfiguration _configuration;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="config">The configuration of the application</param>
        public static void Initialize(IConfiguration config) {
            _configuration = config;
            initializeClient();
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Send a email to the user (Verify your email)
        /// </summary>
        /// <param name="email">The email of the receiver</param>
        /// <param name="name">The name of the user</param>
        /// <param name="tokenValidation">The token to verify the email</param>
        public static void sendVerificationToken(string email, string name, string tokenValidation)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(_configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = getBodySendTokenEmailVerification(name, tokenValidation);
            msg.IsBodyHtml = true;
            msg.Subject = "Confirmación de correo electrónico";

            _client.Send(msg);
        }

        /// <summary>
        /// Send a email to the user (Recuperate your password)
        /// </summary>
        /// <param name="email">The email of the receiver</param>
        /// <param name="name">The name of the user</param>
        /// <param name="passwordToken">The token to recuperate the password</param>
        public static void sendVerificationPasswordToken(string email, string name, string passwordToken)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(_configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = getBodySendTokenChangePassword(name, email, passwordToken);
            msg.IsBodyHtml = true;
            msg.Subject = "Recuperar contraseña";

            _client.Send(msg);
        }

        /// <summary>
        /// Send a email to the user (The user has been banned)
        /// </summary>
        /// <param name="email">The email of the receiver</param>
        /// <param name="name">The name of the user</param>
        /// <param name="ban">True if the user has been banned, false otherwise</param>
        public static void sendBanNotification(string email, string name, bool ban)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(_configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = ban ? getBodySendBanNotification(name) : getBodySendUnBanNotification(name);
            msg.IsBodyHtml = true;
            msg.Subject = ban ? "!Tu cuenta de usuario de VirtualBet ha sido bloqueada!" : "Tu cuenta de VirtualBet ha sido desbloqueada!";

            _client.Send(msg);
        }

        /// <summary>
        /// Send a email to the user (An admin has started a dm with you)
        /// </summary>
        /// <param name="email">The email of the receiver</param>
        /// <param name="name">The name of the user</param>
        /// <param name="dmTitle">The title of the dm conversation</param>
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

        /// <summary>
        /// Send a email to the user (A dm conversation has been close/reopened)
        /// </summary>
        /// <param name="email">The email of the receiver</param>
        /// <param name="name">The name of the user</param>
        /// <param name="dmTitle">The title of the dm conversation</param>
        /// <param name="open">True if the dm has been reopened, false if the dm hass been closed</param>
        public static void sendOpenCloseDMNotification(string email, string name, string dmTitle, bool open)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(_configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = open ? getBodySendOpenDMNotification(name, dmTitle) : getBodySendCloseDMNotification(name, dmTitle);
            msg.IsBodyHtml = true;
            msg.Subject = open ? "Un administrador ha abierto de nuevo una conversación" : "Un administrador ha cerrado una conversación";

            _client.Send(msg);
        }

        /// <summary>
        /// Send a email to the user (A user has spoke in the dm conversation)
        /// </summary>
        /// <param name="email">The email of the receiver</param>
        /// <param name="name">The name of the user</param>
        /// <param name="dmTitle">The title of the dm conversation</param>
        public static void sendOpenCreateDMNotification(string email, string name, string dmTitle)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(_configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = getBodySendCreateDMNotificacion(name, dmTitle);
            msg.IsBodyHtml = true;
            msg.Subject = "Un administrador ha abierto una conversación";

            _client.Send(msg);
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Initializes the smtp client
        /// </summary>
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

        /// <summary>
        /// Get the body for the email (Verify your email)
        /// </summary>
        /// <param name="name">The name of the user who have to verify his email</param>
        /// <param name="tokenVerfication">The token to verify the email</param>
        /// <returns>The body of the email</returns>
        private static String getBodySendTokenEmailVerification(string name, string tokenVerfication)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Haz click en el enlace de abajo para confirmar tu correo electrónico: </p>";
            body += "<a href=\""+_configuration["URL"]+"/emailVerification/" + tokenVerfication+"\"> Confirmar correo electrónico</a>";
            body += "</body></html>";
            return body;
        }

        /// <summary>
        /// Get the body for the email (Recuperate your password)
        /// </summary>
        /// <param name="name">The name of the user who have to recuperate the password</param>
        /// <param name="tokenPassword">The token to recuperate the password</param>
        /// <returns>The body of the email</returns>
        private static String getBodySendTokenChangePassword(string name, string email, string tokenPassword)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Has pedido una nueva contraseña para la cuenta de VirtualBet asociada al email ("+email+"): </p>";
            body += "<p>Si realmente pediste una nueva contraseña sigue el siguiente enlace: </p>";
            body += "<a href=\""+_configuration["URL"]+ "/changePassword/" + tokenPassword+"\">Recuperar contraseña</a>";
            body += "<br><p>Si no fuiste tu quien hizo esta petición, ignora el enlace y toma medidas de seguridad.</p>";
            body += "</body></html>";
            return body;
        }

        /// <summary>
        /// Get the body for the email (The user has been banned)
        /// </summary>
        /// <param name="name">The name of the user who has been banned</param>
        /// <returns>The body of the email</returns>
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

        /// <summary>
        /// Get the body for the email (The user has been unbanned)
        /// </summary>
        /// <param name="name">The name of the user who has been unbanned</param>
        /// <returns>The body of the email</returns>
        private static String getBodySendUnBanNotification(string name)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Los administradores de la plataforma han desbloqueado tu cuenta</p>";
            body += "<p>Puedes volver a acceder a todas sus funcionalidades con normalidad.</p>";
            body += "</body></html>";
            return body;
        }

        /// <summary>
        /// Get the body for the email (The user has been unbanned)
        /// </summary>
        /// <param name="name">The name of the user who has been unbanned</param>
        /// <param name="dmTitle">The title of the dm conversation</param>
        /// <returns>The body of the email</returns>
        private static String getBodySendDMNotification(string name, string dmTitle)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Uno de los administradores de la plataforma te ha contestado en la ";
            body += "conversación sobre el tema " + dmTitle + "</p>";
            body += "</body></html>";
            return body;
        }

        /// <summary>
        /// Get the body for the email (A dm conversation has been reopened)
        /// </summary>
        /// <param name="name">The name of the user who has in the dm</param>
        /// <param name="dmTitle">The title of the dm conversation</param>
        /// <returns>The body of the email</returns>
        private static String getBodySendOpenDMNotification(string name, string dmTitle)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Uno de los administradores de la plataforma ha abierto de nuevo la ";
            body += "conversación sobre el tema " + dmTitle + "</p>";
            body += "</body></html>";
            return body;
        }

        /// <summary>
        /// Get the body for the email (A dm conversation has been closed)
        /// </summary>
        /// <param name="name">The name of the user who has in the dm</param>
        /// <param name="dmTitle">The title of the dm conversation</param>
        /// <returns>The body of the email</returns>
        private static String getBodySendCloseDMNotification(string name, string dmTitle)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Uno de los administradores de la plataforma ha cerrado la ";
            body += "conversación sobre el tema " + dmTitle + "</p>";
            body += "<p>Si tienes otro problema no dudes en abrir otra conversación.</p>";
            body += "</body></html>";
            return body;
        }
        
        /// <summary>
        /// Get the body for the email (A dm conversation has been created)
        /// </summary>
        /// <param name="name">The name of the user who received the dm conversation</param>
        /// <param name="dmTitle">The title of the dm conversation</param>
        /// <returns>The body of the email</returns>
        private static String getBodySendCreateDMNotificacion(string name, string dmTitle)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hola "+name+"</h1><br>";
            body += "<p>Uno de los administradores de la plataforma ha creado una ";
            body += "conversación sobre el tema " + dmTitle + "</p>";
            body += "<p>Dirígite a la plataforma lo más rápido posible y continua la conversación</p>";
            body += "</body></html>";
            return body;
        }
    }
}
