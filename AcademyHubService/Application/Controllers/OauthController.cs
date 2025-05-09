using Application.Services.CalendarService;
using Application.Services.CalendarService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Application.Controllers
{
    public class OauthController(IOptions<CalendarSetting> options, ILogger<CalendarSetting> logger, ICalendarService calendarService) : Controller
    {
        private readonly CalendarSetting _calendarSetting = options.Value;
        private readonly ILogger<CalendarSetting> _logger = logger;
        private readonly ICalendarService _calendarService = calendarService;

        public async Task Callback(string code, string state)
        {
            await _calendarService.GetTokenAsync(code);
        }
    }
}
