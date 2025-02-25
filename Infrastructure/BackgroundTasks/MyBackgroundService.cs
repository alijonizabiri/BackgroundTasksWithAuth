using Infrastructure.Data;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundTasks;

public class MyBackgroundService(ILogger<MyBackgroundService> logger, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation($"My background task is started: {DateTime.UtcNow}");
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            var usersToDelete = await dbContext.Users.Where(u => !u.EmailConfirmed).ToListAsync(stoppingToken);
            if (usersToDelete.Count > 0)
            {
                dbContext.Users.RemoveRange(usersToDelete);
                await dbContext.SaveChangesAsync(stoppingToken);
                logger.LogInformation($"Users deleted: {usersToDelete.Count} ({DateTime.UtcNow})");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); //  Har 1 minut pas kor kna
        }
    }
}
