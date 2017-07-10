using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uzhik.Exceptions
{
        public class MoreThanOneSuitableUserException : System.Exception
        {
            public MoreThanOneSuitableUserException(System.Exception innerException) : base("", innerException)
            {
            }

            public MoreThanOneSuitableUserException(string message) : base(message) { }

            public MoreThanOneSuitableUserException() : base("Найден больше чем один подходящий для авторизации пользователь.") { }
        }
}