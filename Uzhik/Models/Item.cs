using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uzhik.Models
{

    public class Item
    {

        public Item()
        {

        }
        public int ItemId { get; set; }

        public Shop Shop { get; set; }

        public string Name { get; set; }

        public string Link { get; set; }

        public string PicturePath { get; set; }

        public decimal Price { get; set; }

        public DateTime UpdatingTime { get; set; }

        public ItemTracingSettings ItemTracingSettings { get; set; }


    }
}