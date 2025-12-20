using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;
using odev1.Services;
using TrainerServiceImpl = odev1.Services.TrainerService;

// saat dilimi enable
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<UserDetails>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()     //ROL SERV�S� AKT�F
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();   
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();   
builder.Services.AddScoped<ITrainerService, TrainerServiceImpl>();   
builder.Services.AddScoped<ISchedulingQueryService, SchedulingQueryService>();  
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<odev1.Services.TrainerManageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    //roller
    var roles = new[] { "user", "admin", "trainer" };

    foreach (var role in roles)
    {
        //rol yoksa olu�tur
        try
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        catch (Exception ex)
        {
            // logla ama uygulamay� d���rme
        }
    }
}

app.Run();
