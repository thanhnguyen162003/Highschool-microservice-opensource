using Algolia.Search.Http;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Features.FlashcardContentFeature.Commands;
using Application.Services.AIService;
using Application.Services.CalendarService;
using Application.Services.CalendarService.Interface;
using Carter;
using CloudinaryDotNet.Actions;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using UglyToad.PdfPig.Core;

namespace Application.Endpoints
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
