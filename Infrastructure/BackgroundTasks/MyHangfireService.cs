using Hangfire.Logging;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundTasks;

public class MyHangfireService(IServiceScopeFactory serviceScopeFactory, ILogger<MyHangfireService> logger)
{
    public async Task DeleteUsers()
    {
        logger.LogInformation($"My hangfire service is started: {DateTime.UtcNow}");
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var usersToDelete = await dbContext.Users.Where(u => !u.EmailConfirmed).ToListAsync();
        if (usersToDelete.Count <= 0) return;
        
        dbContext.Users.RemoveRange(usersToDelete);
        await dbContext.SaveChangesAsync();
        logger.LogInformation($"Users deleted: {usersToDelete.Count} ({DateTime.UtcNow})");
    }
}
