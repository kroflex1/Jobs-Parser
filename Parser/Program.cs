using System.Reflection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Parser;
using Parser.Services;
using Parser.Services.VacancyParsers;
using Parser.Services.XlsxGenerators;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();

// Регистрация сервисов
builder.Services.AddHttpClient<VacancyParserService>();
builder.Services.AddSingleton<XlsxGeneratorService>();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<SiteParseRulesService>();

// Добавляем Swagger для документирования API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавление MongoClient как singleton
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    MongoDbSettings settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Добавьте Swagger и подключите XML-документацию
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


var app = builder.Build();

// Настройка Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();