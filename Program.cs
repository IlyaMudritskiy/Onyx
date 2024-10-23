using Microsoft.EntityFrameworkCore;
using Onyx.DbContext;
using Onyx.Services;
using Onyx.SignalR;
using Onyx.Startup;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(5000);
});

builder.Services.RegisterServices(builder.Configuration);

var logger = new LoggerConfiguration()
    //.WriteTo.Console()
    .WriteTo.File("Logs/onyx.log", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<UserIdentitiesContext>();
        context.Database.Migrate(); // This applies pending migrations
    }
    catch (Exception ex)
    {
        // Handle exceptions (log errors, etc.)
        logger.Error(ex, "An error occurred while migrating the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors();
app.MapHub<DutUpdateHub>("/newdata-hub");
app.Run();
