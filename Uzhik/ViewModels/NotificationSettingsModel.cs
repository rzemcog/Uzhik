using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uzhik.Models;

namespace Uzhik.ViewModels
{
    public class NotificationSettingsModel
    {
        public Product Product { get; set; }

        public NotificationSettings NotificationSettings { get; set; }
    }
}
