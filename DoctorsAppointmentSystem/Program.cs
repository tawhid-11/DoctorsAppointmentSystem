using DoctorsAppointmentSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using DoctorsAppointmentSystem.Data; // <-- Make sure this points to ApplicationDbContext namespace

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Optional: configure login paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 3. Add MVC services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. Seed Roles and Default Admin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Doctor", "Patient" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string adminEmail = "admin@clinic.com";
    string adminPassword = "Admin@123";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 5. Authentication & Authorization
app.UseAuthentication(); // must come before UseAuthorization
app.UseAuthorization();

// 6. Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedData.Initialize(context);
}


app.Run();
