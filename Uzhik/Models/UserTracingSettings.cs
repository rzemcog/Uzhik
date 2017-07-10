using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uzhik.Models
{
    public enum NotificationMethod
    {
        MobileNumber,
        VKPage,
        Email
    }

    public class UserTracingSettings
    {

        public UserTracingSettings()
        {

        }
        public int UpdatingDataFrequencyInMinutes { get; set; }

        public List<NotificationMethod> NotificationMethods { get; set; }

    }
}