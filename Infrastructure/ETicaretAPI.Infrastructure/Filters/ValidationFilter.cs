using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter //Actiona gelen isteklerde devreye giren filter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                //Validatasyon hatası olan property i bulup onun karşılığında mesajları yazarız
                var errors = context.ModelState
                        .Where(x => x.Value.Errors.Any())
                        .ToDictionary(e => e.Key, e => e.Value.Errors.Select(e => e.ErrorMessage))
                        .ToArray();

                context.Result = new BadRequestObjectResult(errors);
                return; //Bir sonreki filtere geçmek için return etmek gerek
            }

            await next();  //Bundan sonraki filter çalışssın
        }
    }
}
