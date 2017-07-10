using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uzhik.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Uzhik.Controllers
{
    public class AuthorizationController : Controller
    {
        public ActionResult Authorization()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SignIn(User user)
        {
            if (await Models.User.CheckDataIn(user) == Access.Enabled)
            {
                ViewBag.User = user;
                return View("~/Views/Home/Index.cshtml");
            }
            else
            {
                ViewBag.ErrorMessage = "Неверные входные данные!";
                return View("~/Views/Authorization/Authorization.cshtml");
            }
        }

    }
}