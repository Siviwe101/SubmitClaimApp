using Microsoft.EntityFrameworkCore;
using SubmitClaims.Models;
using SubmitClaims.Data;
using Microsoft.AspNetCore.Identity;
using SubmitClaims.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SubmitClaimsContextConnection") 
                       ?? throw new InvalidOperationException("Connection string 'SubmitClaimsContextConnection' not found.");

// Register SubmitClaimsContext with SQLite
builder.Services.AddDbContext<SubmitClaimsContext>(options =>
    options.UseSqlite(connectionString));

// Register ClaimDbContext with SQLite as well
builder.Services.AddDbContext<ClaimDbContext>(options =>
    options.UseSqlite("Data Source=claims.db"));

// Register Identity services with SubmitClaimsContext
builder.Services.AddDefaultIdentity<SubmitClaimsUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<SubmitClaimsContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

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
    pattern: "{controller=SubmitClaim}/{action=Create}/{id?}");

app.MapRazorPages();
app.Run();