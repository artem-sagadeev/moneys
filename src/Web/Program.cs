using ApplicationServices.Identity;
using ApplicationServices.Operations;
using Identity.Data;
using Identity.Entities;
using Identity.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using Operations.Logic.Cards;
using Operations.Logic.Incomes;
using Operations.Logic.Payments;
using ShoppingLists.Data;
using ShoppingLists.Logic.ListItems;
using ShoppingLists.Logic.ShoppingLists;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Environment.EnvironmentName == "Development"
    ? builder.Configuration.GetConnectionString("Local")
    : builder.Configuration.GetConnectionString("Heroku");

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddDbContext<OperationsContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions => 
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
    options.UseNpgsql(connectionString, npgsqlOptions => 
        npgsqlOptions.MigrationsAssembly("Identity"));
});

builder.Services.AddScoped<IShoppingListsService, ShoppingListService>();
builder.Services.AddScoped<IListItemsService, ListItemsService>();
builder.Services.AddDbContext<ShoppingListsContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions => 
        npgsqlOptions.MigrationsAssembly("ShoppingLists"));
});

builder.Services.AddScoped<IOperationsService, OperationsService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler("/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();