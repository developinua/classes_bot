using System.Threading.Tasks;
using Classes.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using ResultNet;

namespace Classes.Application.Providers;

public class UpdateRequestCultureProvider : IRequestCultureProvider
{
    public async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var cultureService = httpContext.RequestServices.GetRequiredService<ICultureService>();
        var updateService = httpContext.RequestServices.GetRequiredService<IUpdateService>();
        
        var update = await updateService.GetUpdateFromHttpRequest(httpContext);
        var username = updateService.GetUsername(update);
        var userCultureCode = await cultureService.GetCodeByUsername(username);

        if (!userCultureCode.IsFailure() && userCultureCode.Data is not null)
            return new ProviderCultureResult(userCultureCode.Data);

        // case before user is registered
        var cultureName = updateService.GetUserCultureName(update.Message);
        var cultureCode = await cultureService.GetCodeByCultureName(cultureName);
        
        return new ProviderCultureResult(cultureCode);
    }
}