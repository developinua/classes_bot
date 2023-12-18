using Classes.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
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
    .UseCustomRequestLocalization();
app.MapControllers();

app.Run();