using ETicaretAPI.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.CustomAttributes
{
    public class AuthorizeDefinitionAttribute : Attribute
    {
        public string Menu { get; set; } //attribute'ın işaretlemiş olduğu action'un hangi menuye ait olduğunu tutmak için kullandık
        public string Definition { get; set; } //Tanımını tutmak için kullandık.
        public ActionType ActionType { get; set; } // Hangi action'a karşılık geldiğini tutmak için kullandık
    }
}
