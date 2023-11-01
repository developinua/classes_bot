using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Classes.Api.Controller;

public class HealthCheckController : ControllerBase
{
    [HttpGet("/health-check")]
    public Task<IResult> Get()
    {
        return Task.FromResult(Results.Ok());
    }
}