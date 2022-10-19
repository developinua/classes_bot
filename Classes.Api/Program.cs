using Classes.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepository();
builder.Services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());

await builder.Services.UseTelegramBotWebHooks(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();