using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    //İçerisinde hangi ürünleri tutacak
    //Hangi ürünlere karşılık gelecek
    //BU ürünlerin kaç tane olduğunun bilgisi de burda tutabilir yani diğer bilgiler
    public class BasketItem : BaseEntity
    {
        public Guid BasketId { get; set; }
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
        public Basket Basket { get; set; }
        public Product Product { get; set; }
    }
}
