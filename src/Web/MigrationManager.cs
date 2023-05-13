using Identity.Data;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using ShoppingLists.Data;

namespace Web;

public static class MigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication webApp)
    {
        using var scope = webApp.Services.CreateScope();
        using var operationsContext = scope.ServiceProvider.GetRequiredService<OperationsContext>();
        using var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        using var shoppingListsContext = scope.ServiceProvider.GetRequiredService<ShoppingListsContext>();

        operationsContext.Database.Migrate();
        identityContext.Database.Migrate();
        shoppingListsContext.Database.Migrate();

        return webApp;
    }
}