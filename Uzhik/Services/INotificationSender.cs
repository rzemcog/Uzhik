using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uzhik.Services
{
    public interface INotificationSender
    {
        void Send(string notificationMessage, string client);
    }
}
