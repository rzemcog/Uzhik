using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uzhik.Models
{
    public class Shop
    {

        public Shop()
        {

        }
        public int ShopId { get; set; }

        public string Name { get; set; }

        public List<Branch> Branches { get; set; }
    }
}