using Classes.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices()
    .AddDatabase(builder.Configuration)
    .AddMediator()
    .AddAutoMapper()
    .AddBotRequests()
    .AddCustomLocalizations()
    .AddControllers()
    .AddNewtonsoftJson(options => options.UseMemberCasing());

await builder.Services.SetTelegramBotWebHook(builder.Configuration);

var app = builder.Build();

app
    .UseCustomRequestLocalization()
    .UseHttpsRedirection()
    .UseRouting();
app.MapControllers();

app.Run();