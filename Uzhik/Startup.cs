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
            string connection = Configuration.GetConnectionString("RemoteConnection");
            services.AddTransient(provider =>
            {
                string collectionName = "users";
                return new MongoContext<User>(connection, collectionName);
            });

            services.AddTransient(provider =>
            {
                string collectionName = "items";
                return new MongoContext<Product>(connection, collectionName);
            });


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
