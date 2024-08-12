using Onyx.Services;
using Onyx.SignalR;
using Onyx.Startup;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterServices(builder.Configuration);

var logger = new LoggerConfiguration()
    //.WriteTo.Console()
    .WriteTo.File("Logs/onyx.log", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

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
