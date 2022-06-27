using Identity.Data;
using Identity.Entities;
using Identity.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Logic.Cards;
using Operations.Logic.Incomes;
using Operations.Logic.Payments;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddDbContext<OperationsContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Heroku"), npgsqlOptions => 
        npgsqlOptions.MigrationsAssembly("Operations"));
});

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<IdentityContext>();
builder.Services.AddDbContext<IdentityContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Heroku"), npgsqlOptions => 
        npgsqlOptions.MigrationsAssembly("Identity"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();