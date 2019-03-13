using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class Email
    {
        SmtpClient client;


        public Email() {
            client = new SmtpClient("smtp.gmail.com");
            client.UseDefaultCredentials = false;
            client.Port = 587;
            string email = Environment.GetEnvironmentVariable("email");
            string pass = Environment.GetEnvironmentVariable("emailPassword");
            client.Credentials = new NetworkCredential(email, pass);
            client.EnableSsl = true;
        }

        public void sendVerificationToken(string email, string name, string tokenValidation)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(Environment.GetEnvironmentVariable("email") + "@gmail.com");
            msg.To.Add(email);
            msg.Body = getBodySendToken(name, tokenValidation);
            msg.IsBodyHtml = true;
            msg.Subject = "Email confirmation";

            client.Send(msg);
        }

        private String getBodySendToken(string name, string tokenVerfication)
        {
            string body = "<html><head></head><body>";
            body += "<p>Hello "+name+"</p>";
            body += "<p>Click on the link below to confirm your email: </p>";
            body += "url--token-->"+tokenVerfication;
            body += "</body></html>";
            return body;
        }
    }
}
