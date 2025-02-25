using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundTasks;

public class DeleteOldUserService(IServiceScopeFactory scopeFactory, ILogger<DeleteOldUserService> logger)
{
    public async Task DeleteOldUsers()
    {
        logger.LogInformation($"delete old users started {DateTime.Now}");

        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        var oldUsers = await dbContext.Users
            .Where(u => u.RegisteredAt.AddMonths(1) <= DateTimeOffset.UtcNow && u.UserName != "admin")
            .ToListAsync();

        if (oldUsers.Count > 0)
        {
            dbContext.RemoveRange(oldUsers);
            await dbContext.SaveChangesAsync();
        }
        logger.LogInformation($"old users are deleted: {oldUsers.Count} - {DateTime.Now}");
    }
}
