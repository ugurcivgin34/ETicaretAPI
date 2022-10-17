using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Repositories.File;
using ETicaretAPI.Application.Repositories.InvoiceFile;
using ETicaretAPI.Application.Repositories.ProductImageFile;
using ETicaretAPI.Domain.Entities.Identy;
using ETicaretAPI.Persistence.Context;
using ETicaretAPI.Persistence.Repositories;
using ETicaretAPI.Persistence.Repositories.File;
using ETicaretAPI.Persistence.Repositories.InvoiceFile;
using ETicaretAPI.Persistence.Repositories.ProductImageFile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{

    //Extension metodlar
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            //DbContext Scoped olarak eklenir arka planda
            services.AddDbContext<ETicaretAPIDbContext>(options => options.UseNpgsql(Configuration.ConnectionString));

            services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<ETicaretAPIDbContext>();

            services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
            services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<IFileReadRepository, FileReadRepository>();
            services.AddScoped<IFileWriteRepository, FileWriteRepository>();
            services.AddScoped<IProductImageFileReadRepository, ProductImageFileReadRepository>();
            services.AddScoped<IProductImageFileWriteRepository, ProductImageFileWriteRepository>();
            services.AddScoped<IInvoiceFileReadRepository, InvoiceFileReadRepository>();
            services.AddScoped<IInvoiceFileWriteRepository, InvoiceFileWriteRepository>();
        }
    }
}
