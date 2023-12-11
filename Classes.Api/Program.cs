using Classes.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services
    .AddAppServices()
    .AddDatabase(configuration)
    .AddMediator()
    .AddAutoMapper()
    .AddBotRequests()
    .AddLocalizations()
    .AddControllers()
    .AddNewtonsoftJson(options => options.UseMemberCasing());

await services.SetTelegramBotWebHook(configuration);

var app = builder.Build();

app
    .UseLocalizations()
    .UseHttpsRedirection();

// app.UseRouting();
app.MapControllers();

app.Run();