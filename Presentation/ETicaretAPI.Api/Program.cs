using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddStorage<LocalStorage>();


builder.Services.AddCors(options =>options.AddDefaultPolicy(policy=>
    policy.WithOrigins("http://localhost:4200","https://localhost:4200").AllowAnyHeader().AllowAnyMethod()
));

builder.Services.AddControllers(options=>options.Filters.Add<ValidationFilter>()) //Bizim olu�turdu�umuz filter
                    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>()) //Bu sayede validator s�n�flarunu otomatik blup i�letebilecez
                    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);//bunu ekleyerek do�rulama hatas� oldu�unda .net core otomatik clienta cevap g�rmesin dedik.Kendi �eylerimizi d�nd�rebiliriz art�k.Defaulttaki filteri kald�rd�k yani
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
