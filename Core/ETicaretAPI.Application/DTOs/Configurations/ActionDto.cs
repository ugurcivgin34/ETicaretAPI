using ETicaretAPI.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.Configurations
{
    public class ActionDto
    {
        public string ActionType { get; set; }
        public string HttpType { get; set; }
        public string Definition { get; set; }
        public string Code { get; set; } // Kullanıcılarla eşleştirme yapacağımız zaman unique olsun diye ve değişiklik göstermesin diye coe tanımlaması yaptık

    }
}
