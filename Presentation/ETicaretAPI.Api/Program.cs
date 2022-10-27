using ETicaretAPI.Api.Configurations.ColumnWriters;
using ETicaretAPI.Api.Extensions;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

builder.Services.AddStorage<LocalStorage>();


builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()
));

//----------------------------------------------------------------Seri log Konfig�rasyon ba�lang��---------------------------------------------------------

Logger log = new LoggerConfiguration()
    .WriteTo.Seq(builder.Configuration["Seq:ServerURL"])
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt")
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostreSQL"), "logs"
    , needAutoCreateTable: true,
    columnOptions: new Dictionary<string, ColumnWriterBase>
    {
        {"message",new RenderedMessageColumnWriter() },
        {"message_template",new MessageTemplateColumnWriter()},
        {"level",new LevelColumnWriter()},
        {"time_stamp",new TimestampColumnWriter()},
        {"exception",new ExceptionColumnWriter() },
        {"log_event", new LogEventSerializedColumnWriter()},
        {"user_name",new UsernameColumnWriter() }

    })
    .Enrich.FromLogContext() //Contexten beslenmesi gerekti�ini anlat�yoruz .Yani a�a��da Authecantion nun alt�nda olu�turdu�umuz context propertisini al usercolumnwriter da ki usermale ile e�le�en varsa elde edebilmi� olsun
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog(log); //Bu servisi �a��rd���m�z zaman builder da logu ile bizim serilogu de�i�tirmi� oluyoruz.

//Request isteklerini log mekanizmalar�na eklemek i�in yapt�
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua"); //Kullan�c�ya ait t�m bilgileri getirir
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;//Requestin ve response in ta��nacak liminit de belirtebiliyoruz
});


//----------------------------------------------------------------Seri log Konfig�rasyon biti�--------------------------------------------------------------



//----------------------------------------------------------------Validasyon Konfig�rasyon ba�lang��--------------------------------------------------------

builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>()) //Bizim olu�turdu�umuz filter
                    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>()) //Bu sayede validator s�n�flarunu otomatik olup i�letebilecez
                    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);//bunu ekleyerek do�rulama hatas� oldu�unda .net core otomatik clienta cevap g�rmesin dedik.Kendi �eylerimizi d�nd�rebiliriz art�k.Defaulttaki filteri kald�rd�k yani

//----------------------------------------------------------------Validasyon Konfig�rasyon ba�lang��--------------------------------------------------------




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//----------------------------------------------------------------JWT Konfig�rasyon ba�lan��-----------------------------------------------------------------


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true, //Olu�turalacak token de�erini kimlerin/hangi originlerin/sitelerin kullan�c� belirledi�imiz de�erdir -> www.bilmemne.com
            ValidateIssuer = true, //Olu�turulacak token de�erini kimin da��tt�n� ifade edece�imiz aland�r.
            ValidateLifetime = true, //Olu�turulan token de�erinin s�resini kontrol edecek olan do�rulamad�r.
            ValidateIssuerSigningKey = true,  //�retilecek token de�erinin uygulmam�za ait bir de�er oldu�unu ifade eden security key verisinin do�rulanmas�d�r.

            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false, //Gelen jwt nin expire s�resi dolmu� ise i�lem yapt�rmayacak

            NameClaimType = ClaimTypes.Name //JWT �zerinde Name claimne kar��l�k gelen de�eri User.Identity.Name propertsinden elde edebiliriz
        };
    });

//----------------------------------------------------------------JWT Konfig�rasyon biti�-----------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>()); //Global exception
app.UseStaticFiles();

app.UseSerilogRequestLogging(); //Bunu nereye koyduysak bundan sonraki middleware lar loglan�r,�ncekiler loglanmaz
app.UseHttpLogging();//Art�k bu uygulamadaki  ya�plan requestleri de log mekanizmas�na eklemi� olduk 
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next();
});

app.MapControllers();

app.Run();
