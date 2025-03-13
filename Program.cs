using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UetdsProgramiNet;
using Web.Modules;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddDbContext<AppDbContext>(m =>
{
    m.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});

// Identity servislerini ekliyoruz
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Password policy settings (isteðe baðlý)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()  // AppDbContext ile Identity'yi entegre ediyoruz
.AddDefaultTokenProviders();  // Varsayýlan token saðlayýcýlarý ekliyoruz

// Kimlik doðrulama yapýlandýrmasýný ekleyin
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie süresi 30 dakika
        options.SlidingExpiration = false; // Süre uzamasýn
    });

// AutoFac entegreasyonu
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// Module Register Etme Tarafý
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

// Middleware sýralamasý - ÖNEMLÝ!
app.UseRouting();           // Ýlk önce routing

app.UseAuthentication();     // Sonra authentication
app.UseAuthorization();      // Sonra authorization

// Son olarak endpoint yapýlandýrmasý
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Referans}/{action=Index}/{id?}");

app.Run();