using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class ProductImageFile : File
    {
        public bool Showcase { get; set; } //Vitrin görseli mi değil mi olduğunu belli etmek için
        public ICollection<Product> Products { get; set; }
    }
}
