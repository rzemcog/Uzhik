using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Scrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Uzhik.Models;
using Uzhik.Services;
using Uzhik.ViewModels;

namespace Uzhik.Controllers
{
    public class AccountController:Controller
    {
        MongoContext<User> _context;
        INotificationSender _notificationSender;
        ScryptEncoder _scryptEncoder;
        public IConfiguration Configuration { get; set; }
        public AccountController(MongoContext<User> context, INotificationSender notificationSender, 
            ScryptEncoder scryptEncoder, IConfiguration configuration)
        {
            _context = context;
            _notificationSender = notificationSender;
            _scryptEncoder = scryptEncoder;
            Configuration = configuration;
        }

        [HttpGet]
        public IActionResult Authorization()
        {
            return View();
        }


        private string GenerateRandomWord()
        {
            Random rnd = new Random();
           
            var length = rnd.Next(20, 40);        
            var str = new StringBuilder(length);
            for(int i=0; i < str.Capacity; i++)
            {
                var temp = rnd.Next(1, 7);
                if (temp <= 4)
                {
                    var temp2 = rnd.Next(1, 5);
                    if (temp2 <= 2) str.Insert(i,(char)rnd.Next(33, 47));
                    else if (temp2 == 3) str.Insert(i, (char)rnd.Next(58, 64));
                    else if (temp2 == 4) str.Insert(i, (char)rnd.Next(91, 96));
                    else str.Insert(i, (char)rnd.Next(123, 126));
                }
                else if (temp == 5)
                    str.Insert(i, (char)rnd.Next(48, 57));
                else if (temp == 6)
                    str.Insert(i, (char)rnd.Next(65, 90));
                else
                    str.Insert(i, (char)rnd.Next(97, 122));
            }
            return str.ToString();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if(ModelState.IsValid)
            {
                var users = await _context.GetCollection();
                var secretWord = Configuration.GetSection("PasswordStrings").GetSection("EndWord").Value;
                User user = users.FirstOrDefault(u => u.Email == loginModel.Email && _scryptEncoder.Compare(loginModel.Password+u.RandomWord+secretWord, u.Password));
                if(user != null)
                {
                    await Authenticate(user.Email);
                    return RedirectToAction("Index", "Main");
                }
                ModelState.AddModelError("", "Некоррекные логин и(или) пароль");

            }
            return View("Authorization");
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
                    var randomWord = GenerateRandomWord();
                    var secretWord = Configuration.GetSection("PasswordStrings").GetSection("EndWord").Value;
                    var password = _scryptEncoder.Encode(registerModel.Password + randomWord + secretWord);
                    user = new User()
                    {
                        Name = registerModel.Name,
                        Password = password,
                        Email = registerModel.Email,
                        RandomWord = randomWord
                    };           
                    _notificationSender.Send("Welcome to our system!", user.Email);
                    await _context.Create(user);
                    await Authenticate(user.Email);
                    return RedirectToAction("Index", "Main");
                }
                else
                    ModelState.AddModelError("", "Данная почта занята");
                   // await _context.Remove(user.Id);
            }
            return View("Authorization");
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
            return RedirectToAction("Authorization", "Account");
        }
    }
}
