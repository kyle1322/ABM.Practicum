using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Timesheets_APP.Data;

var builder = WebApplication.CreateBuilder(args);

// ────────────────
// 1) Configure your two DbContexts
// ────────────────

// Timesheet / business data (Employees, Timesheets, TimesheetsItems, etc)
builder.Services.AddDbContext<TimesheetDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("TimesheetDb"))
);

// Identity user store (AspNetUsers, AspNetRoles, etc)
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ────────────────
// 2) Add ASP.NET Core Identity
// ────────────────
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(opts =>
    {
        opts.SignIn.RequireConfirmedAccount = false;
        // you can relax password rules here if you like
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ────────────────
// 3) Configure the cookie
// ────────────────
builder.Services.ConfigureApplicationCookie(opts =>
{
    // when [Authorize] kicks in, go here:
    opts.LoginPath = "/Account/Admin";
    opts.AccessDeniedPath = "/Account/Admin";
});

// ────────────────
// 4) Add MVC + Razor Pages
// ────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// ────────────────
// 5) Seed the “Admin” role, built-in admin user, and reset kyle-admin
// ────────────────
using (var scope = app.Services.CreateScope())
{
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Ensure "Admin" role exists
    if (!await roleMgr.RoleExistsAsync("Admin"))
        await roleMgr.CreateAsync(new IdentityRole("Admin"));

    // 5A) Seed a built-in "admin" user
    var adminEmail = "admin@you.com";
    var admin = await userMgr.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new IdentityUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true
        };
        var res = await userMgr.CreateAsync(admin, "P@ssw0rd!");
        if (!res.Succeeded)
            throw new Exception("Seeding admin failed: " +
                string.Join(", ", res.Errors.Select(e => e.Description)));
        await userMgr.AddToRoleAsync(admin, "Admin");
    }

    // 5B) Reset the password for kyle-admin (if user exists)
    var kyle = await userMgr.FindByNameAsync("kyle-admin");
    if (kyle != null)
    {
        var token = await userMgr.GeneratePasswordResetTokenAsync(kyle);
        var resetRes = await userMgr.ResetPasswordAsync(kyle, token, "MyNewP@ss!");
        if (!resetRes.Succeeded)
            throw new Exception("Password reset failed: " +
                string.Join(", ", resetRes.Errors.Select(e => e.Description)));
    }
}

// ────────────────
// 6) Middleware pipeline
// ────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication *before* Authorization
app.UseAuthentication();
app.UseAuthorization();

// ────────────────
// 7) Endpoint routing
// ────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapRazorPages();

app.Run();
