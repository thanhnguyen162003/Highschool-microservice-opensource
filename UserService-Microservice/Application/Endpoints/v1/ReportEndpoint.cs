using Application.Common.Models.ReportDocumentModel;
using Application.Features.ReportApp.CreateReport;
using Application.Features.ReportApp.UpdateReport;
using Application.Features.ReportDocument.Commands;
using Application.Features.ReportDocument.Queries;
using Application.Features.User.GetReport;
using Application.Features.User.Statistic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Application.Endpoints.v1
{
    [ApiController]
    [Route("api/v1/reports")]
    [Authorize]
    public class ReportEndpoint(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetReports([FromQuery] GetReportCommand getReportCommand,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(getReportCommand, cancellationToken);

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(result.Metadata));

            return Ok(result.Items);
        }

        [HttpGet("statistic")]
        public async Task<IActionResult> GetReportStatistic(string Type,
           CancellationToken cancellationToken)
        {
            var command = new GetReportStatisticCommand
            {
                Type = Type
            };
            var result = await _sender.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateReport([FromForm] CreateReportCommand createReportCommand, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(createReportCommand, cancellationToken);

            if (result.Status == HttpStatusCode.Created)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

		[HttpGet("document/user")]
		public async Task<IActionResult> GetReportsDocumentUser([FromQuery] ReportDocumentQueryUser getReportCommand,
			CancellationToken cancellationToken)
		{
			var result = await _sender.Send(getReportCommand, cancellationToken);

			Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(result.Metadata));

			return Ok(result.Items);
		}

		[HttpGet("document/admin")]
		public async Task<IActionResult> GetReportsDocumentAdmin([FromQuery] ReportDocumentQueryAdmin getReportCommand,
			CancellationToken cancellationToken)
		{
			var result = await _sender.Send(getReportCommand, cancellationToken);

			Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(result.Metadata));

			return Ok(result.Items);
		}

		[HttpPost("document")]
		public async Task<IActionResult> CreateReportDocument([FromBody] CreateReportDocumentRequestModel createReportDocumentRequestModel, CancellationToken cancellationToken)
		{
            var command = new CreateReportDocumentCommand
			{
				CreateReportDocumentRequestModel = createReportDocumentRequestModel
			};
			var result = await _sender.Send(command, cancellationToken);

			if (result.Status == HttpStatusCode.Created)
			{
				return Ok(result);
			}

			return BadRequest(result);
		}

		[HttpPut("")]
        public async Task<IActionResult> UpdateReport(UpdateReportCommand updateReportCommand, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(updateReportCommand, cancellationToken);

            if (result.Status == HttpStatusCode.OK)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


    }
}
