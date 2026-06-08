using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using ProjetoWeb.Data;
using ProjetoWeb.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURAÇÃO DE SERVIÇOS (Tudo que usa builder.Services)

// Pega a Connection String correta do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ProjetoWebContext")
    ?? throw new InvalidOperationException("Connection string 'ProjetoWebContext' not found.");

// Configura o DbContext usando o MySql (Pomelo)
builder.Services.AddDbContext<ProjetoWebContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        builder => builder.MigrationsAssembly("ProjetoWeb")
    )
);

// Add services to the container
builder.Services.AddControllersWithViews();

// Registrar os Services no sistema (Injeção de Dependência)
builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<SalesRecordService>();


// 2. CONSTRUÇÃO DO APP (Gera a variável 'app')
var app = builder.Build();


// 3. CONFIGURAÇÃO DO PIPELINE DE REQUISIÇÃO (Tudo que usa 'app.Use...')

// Configuração do Localization (Idioma padrão do sistema para en-US)
var enUs = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(enUs),
    SupportedCultures = new List<CultureInfo> { enUs },
    SupportedUICultures = new List<CultureInfo> { enUs }
};
app.UseRequestLocalization(localizationOptions);


// Configurações de ambiente (Development vs Production)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Tela de erro detalhada para o programador

    // Criamos um escopo temporário para chamar o SeedingService com segurança
    using (var scope = app.Services.CreateScope())
    {
        var seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();
        seedingService.Seed(); // Alimenta o banco de dados se estiver vazio
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// O app.Run() A última linha executável do arquivo!
app.Run();