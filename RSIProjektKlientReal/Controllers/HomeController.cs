using JavaEventService;
using Microsoft.AspNetCore.Mvc;
using RSIProjektKlientReal.Models;
using System.Diagnostics;

namespace RSIProjektKlientReal.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventServiceClient _eventServiceClient;

        public HomeController(IConfiguration configuration)
        {
            var endpoint = configuration["EventService:Endpoint"];
            _eventServiceClient = new EventServiceClient(EventServiceClient.EndpointConfiguration.EventServicePort, endpoint);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _eventServiceClient.getAllEventsAsync();
                var events = response.@return?.ToList() ?? new List<@event>();
                return View(events);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Wyst¹pi³ b³¹d podczas pobierania eventów: " + ex.Message;
                return View(new List<@event>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
