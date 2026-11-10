using E_Project.Data;
using E_Project.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles; 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Response Compression — gzip/brotli se pages fast load honge
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "text/css",
        "application/javascript",
        "text/html",
        "application/json",
        "image/svg+xml",
        "text/plain"
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.SmallestSize;
});

var app = builder.Build();

// ✅ Response compression FIRST — sab se pehle enable karo
app.UseResponseCompression();

app.UseHttpsRedirection();

// ✅ AVIF support add kiya
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".avif"] = "image/avif";

// ✅ Static files with caching — 7 days cache for images/css/js
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    OnPrepareResponse = ctx =>
    {
        // Static files ko 7 din tak cache karo
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});

app.UseRouting();
app.UseSession(); // Session middleware

// Add admin navigation lock middleware AFTER session but BEFORE authorization
app.UseMiddleware<AdminNavigationLockMiddleware>();

app.UseAuthorization();

// ✅ SeedData call karo yahan
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    SeedData.Initialize(services);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
