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
    // Password policy settings (iste�e ba�l�)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()  // AppDbContext ile Identity'yi entegre ediyoruz
.AddDefaultTokenProviders();  // Varsay�lan token sa�lay�c�lar� ekliyoruz

// Kimlik do�rulama yap�land�rmas�n� ekleyin
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)  // Default �emay� ekliyoruz
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giri� yap�lacak URL
        options.LogoutPath = "/Account/Logout"; // ��k�� yap�lacak URL
        options.AccessDeniedPath = "/Account/AccessDenied"; // Eri�im reddedildi sayfas�
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie s�resi
        options.SlidingExpiration = true; // Oturum kayboldu�unda s�reyi uzatacak
    });

// AutoFac entegreasyonu
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// Module Register Etme Taraf�
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Kimlik do�rulama ve yetkilendirme middleware'lerini ekliyoruz
app.UseAuthentication(); // Kimlik do�rulama
app.UseAuthorization();  // Yetkilendirme
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
