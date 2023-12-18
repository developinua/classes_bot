using Classes.Application;
using Classes.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddExceptionHandlers()
    .AddAppServices()
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
    .UseExceptionHandler();
app.MapControllers();

await app.MigrateDbAsync();

app.Run();