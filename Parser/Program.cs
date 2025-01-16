using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Parser.Data;
using Parser.Services;
using Parser.Services.ResumeService;
using Parser.Services.VacancyParsers;
using Parser.Services.XlsxGenerators;
using Products.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// CORS с настройкой "разрешить все"
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Разрешает доступ с любого домена
            .AllowAnyMethod()  // Разрешает все методы (GET, POST и т.д.)
            .AllowAnyHeader(); // Разрешает любые заголовки
    });
});

// Добавляем контроллеры
builder.Services.AddControllers();

// Регистрация сервисов
builder.Services.AddHttpClient<VacanciesCollectorService>();
builder.Services.AddSingleton<XlsxVacancyGeneratorService>();
builder.Services.AddHttpClient<ResumeCollectorService>();
builder.Services.AddSingleton<XlsxResumeGeneratorService>();
builder.Services.AddScoped<SiteParseRulesRepository>();

// Добавляем Swagger для документирования API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем DbContext с использованием PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
builder.Services.AddScoped<ISiteParseRulesRepository, SiteParseRulesRepository>();

// Добавьте Swagger и подключите XML-документацию
builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


WebApplication app = builder.Build();

// Настройка Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();