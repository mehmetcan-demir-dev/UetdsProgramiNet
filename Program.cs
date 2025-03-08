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
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)  // Default þemayý ekliyoruz
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giriþ yapýlacak URL
        options.LogoutPath = "/Account/Logout"; // Çýkýþ yapýlacak URL
        options.AccessDeniedPath = "/Account/AccessDenied"; // Eriþim reddedildi sayfasý
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie süresi
        options.SlidingExpiration = true; // Oturum kaybolduðunda süreyi uzatacak
    });

// AutoFac entegreasyonu
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// Module Register Etme Tarafý
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Kimlik doðrulama ve yetkilendirme middleware'lerini ekliyoruz
app.UseAuthentication(); // Kimlik doðrulama
app.UseAuthorization();  // Yetkilendirme
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
