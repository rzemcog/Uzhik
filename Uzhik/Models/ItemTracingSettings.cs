using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uzhik.Models
{
    public enum NotificationType
    {
        Price,
        Presence
    }
    public class ItemTracingSettings
    {

        public ItemTracingSettings()
        {

        }
        public string BranchAddress { get; set; }

        public decimal? RequiredPrice { get; set; } 

        public List<NotificationType> NotificationType {get; set;}


            
    }
}