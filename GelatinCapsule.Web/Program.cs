using System.Globalization;
using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Infrastructure.Persistence;
using GelatinCapsule.Infrastructure.Persistence.Seed;
using GelatinCapsule.Infrastructure.Services;
using GelatinCapsule.Web.Authorization;
using GelatinCapsule.Web.Infrastructure;
using GelatinCapsule.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============ Culture ============
// از InvariantCulture استفاده می‌کنیم که اعشارش نقطه‌ست
// کاربر می‌تونه هم نقطه و هم ویرگول وارد کنه (با ModelBinder سفارشی)
var invariantCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = invariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = invariantCulture;

// ============ Connection String ============
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ============ DbContext ============
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql =>
    {
        sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        sql.EnableRetryOnFailure(3);
    }));

// ============ Cache ============
builder.Services.AddMemoryCache();

// ============ Application Services ============
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IDataScopeService, DataScopeService>();
builder.Services.AddScoped<IReleasePermissionService, ReleasePermissionService>();
builder.Services.AddHttpContextAccessor();

// ============ Cookie Authentication ============
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "GelatinCapsule.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// ============ Authorization ============
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// ============ MVC + Custom Decimal ModelBinder ============
builder.Services.AddControllersWithViews(options =>
{
    // DecimalModelBinder رو در ابتدای لیست می‌ذاریم که اولین binder باشه
    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
});

var app = builder.Build();

// ============ Seed Database ============
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await ApplicationDbSeeder.SeedAsync(db);
}

// ============ Pipeline ============
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();