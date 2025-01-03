using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Parser.Data;
using Parser.Services;
using Parser.Services.VacancyParsers;
using Parser.Services.XlsxGenerators;
using Products.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();

// Регистрация сервисов
builder.Services.AddHttpClient<VacanciesCollectorService>();
builder.Services.AddSingleton<XlsxVacancyGeneratorService>();
builder.Services.AddScoped<SiteParseRulesService>();

// Добавляем Swagger для документирования API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем DbContext с использованием PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
builder.Services.AddScoped<ISiteParseRulesService, SiteParseRulesService>();

// Добавьте Swagger и подключите XML-документацию
builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});



WebApplication app = builder.Build();

// Настройка Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();