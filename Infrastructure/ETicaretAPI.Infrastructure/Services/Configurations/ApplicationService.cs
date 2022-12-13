using Azure.Storage.Blobs.Models;
using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.Configurations;
using ETicaretAPI.Application.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Configurations
{
    public class ApplicationService : IApplicationService
    {
        public List<MenuDto> GetAuthorizeDefinitionEndpoints(Type type)
        {
            Assembly assembly = Assembly.GetAssembly(type); //Şuan çalışan type'ı verilmiş hangi assemblye varsa onu getirecek.Örneğin controllerda çalışma yapılıyorsa ordakiler gelecek
            var controllers = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ControllerBase))); //ControllerBase sınıfından ne kadar türüyen varsa bul getir

            List<MenuDto> menus = new();
            if (controllers != null)
                foreach (var controller in controllers)
                {
                    var actions = controller.GetMethods().Where(m => m.IsDefined(typeof(AuthorizeDefinitionAttribute)));
                    if (actions != null)
                        foreach (var action in actions)
                        {
                            var attributes = action.GetCustomAttributes(true);
                            if (attributes != null)
                            {
                                MenuDto menu = new();

                                var authorizeDefinitionAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;
                                if (!menus.Any(m => m.Name == authorizeDefinitionAttribute.Menu))
                                {
                                    menu = new() { Name = authorizeDefinitionAttribute.Menu };
                                    menus.Add(menu);
                                }
                                else
                                    menu = menus.FirstOrDefault(m => m.Name == authorizeDefinitionAttribute.Menu);

                                ActionDto _action = new()
                                {
                                    ActionType = Enum.GetName(typeof(ActionType),authorizeDefinitionAttribute.ActionType),
                                    Definition = authorizeDefinitionAttribute.Definition
                                };
                                var httpAttribute = attributes.FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) as HttpMethodAttribute; //HttpMethodAttribute den kalıtım almış mı
                                //dönen değer obje olarak döneceği için as yarak HttpMethodAttribute döndürdük

                                if (httpAttribute != null)
                                    _action.HttpType = httpAttribute.HttpMethods.First(); //İlk değeri string olarak getirecektir
                                else
                                    _action.HttpType = HttpMethods.Get;

                                _action.Code = $"{_action.HttpType}.{_action.ActionType}.{_action.Definition.Replace(" ", "")}";

                                menu.Actions.Add(_action);

                            }
                        }
                }


            return menus;
        }
    }
}



