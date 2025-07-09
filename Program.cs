using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Timesheets_APP.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TimesheetDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("TimesheetDb"))
);

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(opts =>
    {
        opts.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Account/Admin";
    opts.AccessDeniedPath = "/Account/Admin";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleMgr.RoleExistsAsync("Admin"))
        await roleMgr.CreateAsync(new IdentityRole("Admin"));

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
            throw new Exception(
                "Seeding admin failed: " +
                string.Join(", ", res.Errors.Select(e => e.Description))
            );
        await userMgr.AddToRoleAsync(admin, "Admin");
    }

    var kyle = await userMgr.FindByNameAsync("kyle-admin");
    if (kyle != null)
    {
        var token = await userMgr.GeneratePasswordResetTokenAsync(kyle);
        var resetRes = await userMgr.ResetPasswordAsync(kyle, token, "MyNewP@ss!");
        if (!resetRes.Succeeded)
            throw new Exception(
                "Password reset failed: " +
                string.Join(", ", resetRes.Errors.Select(e => e.Description))
            );
    }
}

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
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapRazorPages();

app.Run();
