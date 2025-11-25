using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using ValidadorSenhaSegura.Application;
using ValidadorSenhaSegura.Application.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    // Define a versão padrão da API.
    options.DefaultApiVersion = new ApiVersion(2, 0);

    options.AssumeDefaultVersionWhenUnspecified = true;

    // Envia cabeçalhos mostrando versões suportadas
    options.ReportApiVersions = true;

    // Somente via Header
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";

    // não usa URL
    options.SubstituteApiVersionInUrl = false;
});

builder.Services.AddInfrastructure();
builder.Services.AddDomain();
builder.Services.AddApplication();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Register Swagger generation and operation filter (do NOT build a service provider here)
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<ApiVersionHeaderOperationFilter>();
});

// Provide SwaggerDoc entries via DI after IApiVersionDescriptionProvider is available
builder.Services.AddTransient<
    Microsoft.Extensions.Options.IConfigureOptions<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions>, 
    ValidadorSenhaSegura.Application.Configuration.ConfigureSwaggerOptions>();

var app = builder.Build();

app.UseGlobalExceptionHandler(); // Middleware criado para tratativa global de exceções.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    ConfigSwagger(app);
}

app.UseHttpsRedirection();

// --- CRIA ApiVersionSet e PASSA PARA RegisterRoutes --- 
// Usa a extensão definida em Application.Configuration.RegisterApiVersions
var versionSet = app.GetInstanceApiVersionSet();
app.AddRoutes(versionSet);

app.Run();

static void ConfigSwagger(WebApplication app)
{
    var providerSwagger = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in providerSwagger.ApiVersionDescriptions)
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"API {description.GroupName}"
            );
    });
}

public partial class Program
{
}