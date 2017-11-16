using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Uzhik.Models;
using Uzhik.ViewModels;

namespace Uzhik.Controllers
{
    public class AccountController:Controller
    {
        MongoContext<User> _context;
        public AccountController(MongoContext<User> context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if(ModelState.IsValid)
            {
                var users = await _context.GetCollection();
                User user = users.FirstOrDefault(u => u.Email == loginModel.Email);
                if(user != null)
                {
                    await Authenticate(user.Email);
                    return RedirectToAction("Index", "Main");
                }
                ModelState.AddModelError("", "Некоррекные логин и(или) пароль");

            }
            return View(loginModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if(ModelState.IsValid)
            {
                var users = await _context.GetCollection();
                User user = users.FirstOrDefault(u => u.Email == registerModel.Email);
                if (user == null)
                {
                    user = new User()
                    {
                        Name = registerModel.Name,
                        Password = registerModel.Password,
                        Email = registerModel.Email
                    };
                    await _context.Create(user);
                    await Authenticate(user.Email);
                    return RedirectToAction("Index", "Main");
                }
                else
                    ModelState.AddModelError("", "Данная почта занята");
            }
            return View(registerModel);
        }

        private async Task Authenticate(string userEmail)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userEmail)
            };

            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
