using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uzhik.Controllers
{
    public class MainController:Controller
    {

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
