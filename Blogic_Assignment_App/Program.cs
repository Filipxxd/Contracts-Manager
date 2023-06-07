using InsuranceApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InsuranceAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InsuranceDb")));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IConsultantService, ConsultantService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IHomeService, HomeService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();