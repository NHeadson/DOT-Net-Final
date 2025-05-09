using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("rename.appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration["Data:Northwind:ConnectionString"]));
builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(configuration["Data:AppIdentity:ConnectionString"]));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    if (!await roleManager.RoleExistsAsync("employee"))
    {
        await roleManager.CreateAsync(new IdentityRole("employee"));
    }

    // Assign the "employee" role to all users
    var users = await userManager.Users.ToListAsync();
    foreach (var user in users)
    {
        if (!await userManager.IsInRoleAsync(user, "employee"))
        {
            await userManager.AddToRoleAsync(user, "employee");
        }
    }

    var employees = await userManager.Users.ToListAsync();
    foreach (var employee in employees)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(employee);
        await userManager.ResetPasswordAsync(employee, token, "B@nanas1"); // Set a new password
    }
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
