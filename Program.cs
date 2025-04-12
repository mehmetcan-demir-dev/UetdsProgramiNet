using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UetdsProgramiNet;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Services;
using Web.Modules;

var builder = WebApplication.CreateBuilder(args);

// DI Container kullanımı (Autofac)
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    containerBuilder.RegisterModule(new RepoServiceModule()));  // RepoServiceModule ile servisleri register et.

// MVC yapılandırması
builder.Services.AddControllersWithViews();

// Servisleri DI konteynırına ekleme
builder.Services.AddScoped<IUserService, UserService>();  // IUserService için UserService implementasyonu

// DbContext ve Identity yapılandırması
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
    });
});

// Identity yapılandırması
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// UserManager ve SignInManager servislerini DI'ye ekleyelim
builder.Services.AddScoped<UserManager<AppUser>>();
builder.Services.AddScoped<SignInManager<AppUser>>();

// Kimlik doğrulama yapılandırması (Cookie Authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Oturum süresi
        options.SlidingExpiration = true;  // Oturum aktifse süresi uzar
    });

// Session yapılandırması
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturum süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;  // Gereklilik olarak tanımlandı
});

var app = builder.Build();

// Hata yakalama ve güvenlik
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Güvenli bağlantı için HSTS kullanımı
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 404 hatası alınan her URL için Bulunamadi sayfasına yönlendir
app.UseStatusCodePagesWithReExecute("/Bulunamadi/Bulunamadi");  // 404 hatasında BulunamadiController'a yönlendir

// Session yapılandırması
app.UseSession();  // Session middleware'ini etkinleştir

// Kimlik doğrulama ve yetkilendirme
app.UseAuthentication();  // Kullanıcı doğrulaması
app.UseAuthorization();   // Kullanıcı yetkilendirmesi

// Route yapılandırması
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
