using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Uzhik.Services
{

    //Сервис по отправке оповощений по email
    public class EmailNotificationSender : INotificationSender
    {

        private readonly string _email;
        SmtpClient smtpClient;

        public EmailNotificationSender(string email, string port, string host, string password)
        {
            _email = email;
            smtpClient = new SmtpClient
            {
                Port = Convert.ToInt32(port),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = host,
                Credentials = new System.Net.NetworkCredential(email, password),
                EnableSsl = true
            };
        }


        public void Send(string notificationMessage, string client)
        {
            MailMessage mail = new MailMessage(_email, client)
            {
                Subject = "UzhikNotification",
                Body = notificationMessage
            };
            smtpClient.Send(mail);
        }

        public static string GenerateNotificationMessage(bool available, bool priceChanging, 
            bool availability, int? price, string productName)
        {
            var message = new StringBuilder();
            message.Append("Уважаемый пользователь!\n");
            if(available && availability)
            {
                message.Append($"Отслеживаемый товар: {productName} теперь в наличии\n");
                if (priceChanging && price != null)
                {
                    message.Append($"и его цена достигла запрошенный отметки в {price} рублей.\n");
                }
                else if (priceChanging && price != null)
                    message.Append($", но его цена не достигла запрошенной отметки в {price} рублей.\n");
            }
            else if(priceChanging && price!=null)
            {
                message.Append($"Цена на отслеживаемый товар {productName} достигла запрошенной отметки в\n");
                message.Append($"{price} рублей");
            }
            return message.ToString();                   
        }

       
    }
}
