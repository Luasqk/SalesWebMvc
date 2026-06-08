using ProjetoWeb.Data;
using Microsoft.EntityFrameworkCore;
using ProjetoWeb.Services;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar o SeedingService no sistema (Injeção de Dependência)
builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();

var app = builder.Build();

// ========================
// CONFIGURAÇÃO DO PIPELINE 
// ========================

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

// O app.Run() DEVE ser a última linha executável do arquivo!
app.Run();