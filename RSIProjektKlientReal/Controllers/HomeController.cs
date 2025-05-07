using JavaEventService;
using Microsoft.AspNetCore.Mvc;
using RSIProjektKlientReal.Models;
using System.Diagnostics;
using System.Globalization;

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

        public async Task<IActionResult> AddOrUpdate(@event e)
        {
            if (DateTime.TryParse(e.date, out var date))
            {
                e.month = date.Month;
                e.year = date.Year;
                e.week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    date,
                    CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
            }
            if(!string.IsNullOrWhiteSpace(e.id))
                await _eventServiceClient.updateEventAsync(e);
            else
                await _eventServiceClient.addEventAsync(e);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(@event e)
        {
            await _eventServiceClient.deleteEventAsync(e.id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetByDate(string date)
        {
            try
            {
                var response = await _eventServiceClient.getEventsForDayAsync(date);
                var events = response.@return?.ToList() ?? new List<@event>();
                return View("Index", events);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "B³¹d podczas pobierania eventów: " + ex.Message;
                return View("Index", new List<@event>());
            }
        }

        public async Task<IActionResult> GetByMonth(int month, int year)
        {
            try
            {
                var response = await _eventServiceClient.getEventsForMonthAsync(year, month);
                var events = response.@return?.ToList() ?? new List<@event>();
                return View("Index", events);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "B³¹d podczas pobierania eventów: " + ex.Message;
                return View("Index", new List<@event>());
            }
        }

        public async Task<IActionResult> GetByWeek(int week, int year)
        {
            try
            {
                var response = await _eventServiceClient.getEventsForWeekAsync(year, week);
                var events = response.@return?.ToList() ?? new List<@event>();
                return View("Index", events);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "B³¹d podczas pobierania eventów: " + ex.Message;
                return View("Index", new List<@event>());
            }
        }

        public async Task<IActionResult> GetByID(string id)
        {
            try
            {
                var response = await _eventServiceClient.getEventByIdAsync(id);
                var events = new List<@event>();

                if (response.@return != null)
                {
                    events.Add(response.@return);
                }

                return View("Index", events);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "B³¹d podczas pobierania eventów: " + ex.Message;
                return View("Index", new List<@event>());
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
