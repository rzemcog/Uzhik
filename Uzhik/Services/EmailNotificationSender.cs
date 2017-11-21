using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Uzhik.Services
{

    //Сервис по отправке оповощений по email
    public class EmailNotificationSender : INotificationSender
    {

        private readonly string _email;
        private readonly string _client;
        SmtpClient smtpClient;

        public EmailNotificationSender(string email, string client)
        {
            _email = email;
            _client = client;
            smtpClient = new SmtpClient
            {
                Port = 465,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "smtp.mail.ru",
                Credentials = new System.Net.NetworkCredential("rzemcog@mail.ru", "r18349276g"),
                EnableSsl = true
            };
        }


        public void Send(string notificationMessage)
        {
            MailMessage mail = new MailMessage(_email, _client)
            {
                Subject = "Оповещение",
                Body = notificationMessage
            };
            smtpClient.Send(mail);
        }

       
    }
}
