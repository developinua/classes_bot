using Classes.Domain.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services
    .AddAppServices(configuration)
    .AddControllers()
    .AddNewtonsoftJson(options => options.UseMemberCasing());

await services.UseTelegramBotWebHooks(configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();