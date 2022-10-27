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

//----------------------------------------------------------------Seri log Konfigürasyon baþlangýç---------------------------------------------------------

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
    .Enrich.FromLogContext() //Contexten beslenmesi gerektiðini anlatýyoruz .Yani aþaðýda Authecantion nun altýnda oluþturduðumuz context propertisini al usercolumnwriter da ki usermale ile eþleþen varsa elde edebilmiþ olsun
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog(log); //Bu servisi çaðýrdýðýmýz zaman builder da logu ile bizim serilogu deðiþtirmiþ oluyoruz.

//Request isteklerini log mekanizmalarýna eklemek için yaptý
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua"); //Kullanýcýya ait tüm bilgileri getirir
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;//Requestin ve response in taþýnacak liminit de belirtebiliyoruz
});


//----------------------------------------------------------------Seri log Konfigürasyon bitiþ--------------------------------------------------------------



//----------------------------------------------------------------Validasyon Konfigürasyon baþlangýç--------------------------------------------------------

builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>()) //Bizim oluþturduðumuz filter
                    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>()) //Bu sayede validator sýnýflarunu otomatik olup iþletebilecez
                    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);//bunu ekleyerek doðrulama hatasý olduðunda .net core otomatik clienta cevap görmesin dedik.Kendi þeylerimizi döndürebiliriz artýk.Defaulttaki filteri kaldýrdýk yani

//----------------------------------------------------------------Validasyon Konfigürasyon baþlangýç--------------------------------------------------------




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//----------------------------------------------------------------JWT Konfigürasyon baþlanýç-----------------------------------------------------------------


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true, //Oluþturalacak token deðerini kimlerin/hangi originlerin/sitelerin kullanýcý belirlediðimiz deðerdir -> www.bilmemne.com
            ValidateIssuer = true, //Oluþturulacak token deðerini kimin daðýttýný ifade edeceðimiz alandýr.
            ValidateLifetime = true, //Oluþturulan token deðerinin süresini kontrol edecek olan doðrulamadýr.
            ValidateIssuerSigningKey = true,  //Üretilecek token deðerinin uygulmamýza ait bir deðer olduðunu ifade eden security key verisinin doðrulanmasýdýr.

            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false, //Gelen jwt nin expire süresi dolmuþ ise iþlem yaptýrmayacak

            NameClaimType = ClaimTypes.Name //JWT üzerinde Name claimne karþýlýk gelen deðeri User.Identity.Name propertsinden elde edebiliriz
        };
    });

//----------------------------------------------------------------JWT Konfigürasyon bitiþ-----------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>()); //Global exception
app.UseStaticFiles();

app.UseSerilogRequestLogging(); //Bunu nereye koyduysak bundan sonraki middleware lar loglanýr,öncekiler loglanmaz
app.UseHttpLogging();//Artýk bu uygulamadaki  yaýplan requestleri de log mekanizmasýna eklemiþ olduk 
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
