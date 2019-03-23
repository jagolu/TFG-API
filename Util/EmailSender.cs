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
            msg.Body = getBodySendToken(name, tokenValidation);
            msg.IsBodyHtml = true;
            msg.Subject = "Email confirmation";

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

        private static String getBodySendToken(string name, string tokenVerfication)
        {
            string body = "<html><head></head><body>";
            body += "<h1>Hello "+name+"</h1><br>";
            body += "<p>Click on the link below to confirm your email: </p>";
            body += _configuration["URL"]+"/emailVerification/" + tokenVerfication;
            body += "</body></html>";
            return body;
        }
    }
}
