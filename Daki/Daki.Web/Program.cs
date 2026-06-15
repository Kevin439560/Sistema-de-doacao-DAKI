using Daki.Dominio.Interfaces;
using Daki.Infra.Data;
using Daki.Infra.Repositorios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Configurar o DbContext para usar PostgreSQL
builder.Services.AddDbContext<DakiContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar os repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAnuncioRepository, AnuncioRepository>();
builder.Services.AddScoped<IInteresseRepository, InteresseRepository>();

// Configuração do Cookie de Autenticação
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddControllersWithViews();

// 1. Configuração Blindada do Data Protection
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
{
    var pastaChaves = "/home/site/keys";
    try
    {
        if (!Directory.Exists(pastaChaves)) Directory.CreateDirectory(pastaChaves);
    }
    catch { /* Ignora o bloqueio do SMB da Azure */ }

    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(pastaChaves))
        .SetApplicationName("DakiApp");
}
else
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(@"/app/keys"))
        .SetApplicationName("DakiApp");
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 2. Configuração Blindada das Imagens
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
{
    var pastaPersistenteUploads = Path.Combine("/home/site", "uploads");

    try
    {
        if (!Directory.Exists(pastaPersistenteUploads))
        {
            Directory.CreateDirectory(pastaPersistenteUploads);
        }
    }
    catch (Exception)
    {
        // A pasta é criada, mas o SMB recusa a mudança de permissões do .NET.
        // Capturamos o erro para impedir que a aplicação quebre na inicialização.
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(pastaPersistenteUploads),
        RequestPath = "/uploads"
    });
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DakiContext>();
    context.Database.Migrate();
}

app.Run();