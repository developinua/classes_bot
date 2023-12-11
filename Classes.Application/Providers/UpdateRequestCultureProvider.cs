using System.Threading.Tasks;
using Classes.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Classes.Application.Providers;

public class UpdateRequestCultureProvider(ICultureService cultureService) : IRequestCultureProvider
{
    public async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        // Retrieve the user's identity from the current context (e.g., from a cookie or JWT token)
        var userId = httpContext.User.Identity?.Name; // Replace with your user identification logic
        // todo: check logic for start command(unregistered user) and other commands when user is in db

        // Fetch user's preferred culture from the database
        var userCulture = await cultureService.GetByName("en-us");

        return new ProviderCultureResult(userCulture.LanguageCode);
    }
}