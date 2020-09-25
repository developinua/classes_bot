namespace TarasZoukClasses.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        [HttpGet]
        [Route("{controller}/{action}")]
        public ActionResult Index()
        {
            return Ok("Hi there!");
        }
    }
}
