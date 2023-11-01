using Classes.Domain.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());

services.AddAppServices(builder.Configuration);
await services.UseTelegramBotWebHooks(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();