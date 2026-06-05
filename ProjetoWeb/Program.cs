using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//pega a Connection String correta do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ProjetoWebContext")
    ?? throw new InvalidOperationException("Connection string 'ProjetoWebContext' not found.");

//configura o DbContext usando o MySql (Pomelo)
builder.Services.AddDbContext<ProjetoWebContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        builder => builder.MigrationsAssembly("ProjetoWeb")
    )
);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.Run();