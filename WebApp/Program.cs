using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.BackgroundTasks;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddServices(builder.Configuration);
builder.Services.SwaggerConfigurationServices();
builder.Services.AuthConfigureServices(builder.Configuration);

//builder.Services.AddHostedService<MyBackgroundService>();

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<MyHangfireService>();

var app = builder.Build();
try
{
    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    var datacontext = serviceProvider.GetRequiredService<DataContext>();
    await datacontext.Database.MigrateAsync();
    // seed
    var seeder = serviceProvider.GetRequiredService<Seeder>();
    await seeder.SeedRole();
    await seeder.SeedUser();
    //hangfire
    var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();
    
    recurringJobManager.AddOrUpdate<MyHangfireService>(
        "delete-unverified-users",
        service => service.DeleteUsers(),
        Cron.Hourly
    );
    
    recurringJobManager.AddOrUpdate<DeleteOldUserService>(
        "delete-old-users",
        service => service.DeleteOldUsers(),
        Cron.Minutely
    );
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard();
app.UseRouting();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();