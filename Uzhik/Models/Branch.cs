using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uzhik.Models
{
    public class Branch
    {

        public Branch()
        {

        }

        public Shop Shop {get; set;}

        public string Address { get; set; }
    }
}