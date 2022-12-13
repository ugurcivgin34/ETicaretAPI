using ETicaretAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.Configurations
{
    public interface IApplicationService //Data olarak bir işlem yapmayacağız sadece Authorize işlemleri için konfigürasyon ayarı yapacağız.O yüzden böyle bir yapı kullandık
    {
        List<MenuDto> GetAuthorizeDefinitionEndpoints(Type type);
    }
}
