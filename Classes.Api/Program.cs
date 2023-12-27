using Classes.Application;
using Classes.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddExceptionHandlers()
    .AddAppServices()
    .AddMiddlewares()
    .AddDatabase(builder.Configuration)
    .AddCustomLocalizations()
    .AddMediator()
    .AddAutoMapper()
    .AddBotRequests()
    .AddControllers()
    .AddNewtonsoftJson(options => options.UseMemberCasing());

await builder.Services.SetTelegramBotWebHook(builder.Configuration);

var app = builder.Build();

app
    .UseHttpsRedirection()
    .UseRouting()
    .UseCustomRequestLocalization()
    .UseExceptionHandler()
    .UseMiddlewares();
app.MapControllers();

await app.MigrateDbAsync();

app.Run();