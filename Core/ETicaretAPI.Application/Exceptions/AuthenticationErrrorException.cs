using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Exceptions
{
    public class AuthenticationErrrorException : Exception
    {
        public AuthenticationErrrorException():base("Kimlik doğrulama hatası")
        {
        }

        public AuthenticationErrrorException(string? message) : base(message)
        {
        }

        public AuthenticationErrrorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
