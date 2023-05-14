using ApplicationServices.Identity;
using ApplicationServices.Operations;
using ApplicationServices.ShoppingLists;
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
using Web;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Environment.IsEnvironment(Environments.Development)
    ? builder.Configuration.GetConnectionString("Local")
    : Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!;

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IIncomeRecordService, IncomeRecordService>();
builder.Services.AddScoped<IPaymentRecordService, PaymentRecordService>();
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

builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IListItemService, ListItemService>();
builder.Services.AddDbContext<ShoppingListsContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions => 
        npgsqlOptions.MigrationsAssembly("ShoppingLists"));
});

builder.Services.AddScoped<IOperationsService, OperationsService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IShoppingListsService, ShoppingListsService>();

var app = builder.Build();

if (Environment.GetEnvironmentVariable("AUTOMIGRATE") == "True")
{
    using var scope = app.Services.CreateScope();
    
    var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    identityContext.Database.Migrate();
    
    var operationsContext = scope.ServiceProvider.GetRequiredService<OperationsContext>();
    operationsContext.Database.Migrate();
    
    var shoppingListsContext = scope.ServiceProvider.GetRequiredService<ShoppingListsContext>();
    shoppingListsContext.Database.Migrate();
}

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

app.MigrateDatabase();

app.Run();