using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Uzhik.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Uzhik.Services;
using Scrypt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;

namespace Uzhik
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        //настройка сервисов
        public void ConfigureServices(IServiceCollection services)
        {
            string mongoConnection = Configuration.GetConnectionString("RemoteConnection");
            string msssqlConnection = Configuration.GetConnectionString("MsSQLConnection");
            services.AddTransient(provider =>
            {
                string collectionName = "users";
                return new MongoContext<User>(mongoConnection, collectionName);
            });

            services.AddTransient(provider =>
            {
                string collectionName = "Items";
                return new MongoContext<Product>(mongoConnection, collectionName);
            });

            services.AddTransient<INotificationSender, EmailNotificationSender>(provider =>
            {
                var smtpConfiguration = Configuration.GetSection("SmtpSettings");
                string email = smtpConfiguration.GetSection("Email").Value;
                string host = smtpConfiguration.GetSection("Host").Value;
                string port = smtpConfiguration.GetSection("Port").Value;
                string password = smtpConfiguration.GetSection("Password").Value;
                return new EmailNotificationSender(email, port, host, password);
            });

            //services.AddDbContext<MsSQLContext>(options =>
            //{
            //    options.UseSqlServer(msssqlConnection);
            //});

            services.AddTransient<ScryptEncoder>();

           

            // установка конфигурации подключения
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new PathString("/Account/Authorization");
                });

            services.AddMvc();
        }


        //конвейер обработки запросов MiddleWare(промежуточный слой)
        public void Configure(IApplicationBuilder app)
        {
            IHostingEnvironment env = app.ApplicationServices.GetService<IHostingEnvironment>(); //получаем сервис IHostingEnvironment 
                                                                                                 //через свойство ApplicationServices 
                                                                                                 //объекта IApplicationBuilder
            //Если среда в разработке - выкинуть страницу исключения
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            //Использования приложением файлов по умолчанию и статических файлов
            // app.UseDefaultFiles();
             app.UseStaticFiles();
             app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Main}/{action=Index}/{id?}");
            });
        }
    }
}
