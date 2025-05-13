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

    // Create a test employee account
    if (await userManager.FindByEmailAsync("employee@test.com") == null)
    {
        var employeeUser = new AppUser { UserName = "employee@nwind.com", Email = "employee@nwind.com" };
        var result = await userManager.CreateAsync(employeeUser, "B@nanas1");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(employeeUser, "employee");
        }
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
