using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Enums
{
    //enum ifadeleri hangi katmanda lazımsa oraya koymak daha mantıklı.Bu katmanda kullanacağımız için sadece,domain de koymaya gerek yok,herkes tarafından erişlmeyecek sonuçta
    public enum StorageType
    {
        Local,
        Azure,
        AWS
    }
}
