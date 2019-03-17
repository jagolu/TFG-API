using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    public class Email
    {
        private SmtpClient client;
        private IConfiguration configuration;

        public Email(IConfiguration config) {
            this.configuration = config;
            initializeClient();
        }

        public void sendVerificationToken(string email, string name, string tokenValidation)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(this.configuration["Email:username"] + "@gmail.com");
            msg.To.Add(email);
            msg.Body = getBodySendToken(name, tokenValidation);
            msg.IsBodyHtml = true;
            msg.Subject = "Email confirmation";

            client.Send(msg);
        }

        private void initializeClient()
        {
            client = new SmtpClient("smtp.gmail.com");
            client.UseDefaultCredentials = false;
            client.Port = 587;
            string email = this.configuration["Email:username"];
            string pass = this.configuration["Email:password"];
            client.Credentials = new NetworkCredential(email, pass);
            client.EnableSsl = true;
        }

        private String getBodySendToken(string name, string tokenVerfication)
        {
            string body = "<html><head></head><body>";
            body += "<p>Hello "+name+"</p>";
            body += "<p>Click on the link below to confirm your email: </p>";
            body += this.configuration["URL"]+"/emailVerification/" + tokenVerfication;
            body += "</body></html>";
            return body;
        }
    }
}
