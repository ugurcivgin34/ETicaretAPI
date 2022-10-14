using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities.Common
{
    public class BaseEntity
    {
        public Guid  Id { get; set; }
        public DateTime CreatedDate { get; set; }
        virtual public DateTime UpdatedDate { get; set; } //İlla her entity de bu column ı oluşturmamız gerekmeyebilir.Bu durumda virtual şeklinde belirtip entity de oluşturmak istemediğimizi belirtebiliriz


    }
}
