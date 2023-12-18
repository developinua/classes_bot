using System.IO;
using System.Text;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Classes.Application.Providers;

public class UpdateRequestCultureProvider : IRequestCultureProvider
{
    public async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var userService = httpContext.RequestServices.GetRequiredService<IUserService>();
        var body = await GetBodyFromHttpContext(httpContext);
        var update = GetUpdateFromBody(body);
        var username = update?.Message?.From?.Username;
        var languageCode = update?.Message?.From?.LanguageCode;
        var userCulture = await userService.GetUserCulture(username);

        if (!string.IsNullOrWhiteSpace(userCulture?.LanguageCode))
            return new ProviderCultureResult(userCulture.LanguageCode);

        // case before user is registered
        return string.IsNullOrWhiteSpace(languageCode)
            ? new ProviderCultureResult(new Culture().LanguageCode)
            : new ProviderCultureResult(languageCode);
    }

    private async Task<string?> GetBodyFromHttpContext(HttpContext httpContext)
    {
        // Needed to re-read the stream
        httpContext.Request.EnableBuffering();

        using var reader = new StreamReader(
            httpContext.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        // Reset the stream position for the next middleware
        httpContext.Request.Body.Position = 0;

        return body;
    }
    
    private Update? GetUpdateFromBody(string? body) => JsonConvert.DeserializeObject<Update>(body ?? string.Empty);
}