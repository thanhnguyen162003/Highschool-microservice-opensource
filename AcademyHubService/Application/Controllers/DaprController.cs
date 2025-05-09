using Application.Common.Models.ZoneModel;
using Application.Common.Ultils;
using Application.Features.ZoneFeatures.Commands;
using Application.Features.ZoneFeatures.Queries;
using Domain.Constants;
using Domain.DaprModels;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Controllers
{
    [Route("api/v1/dapr")]
    [ApiController]
    public class DaprController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        [HttpGet("zone-member")]
        [ProducesResponseType(typeof(ZoneResponseDapr), StatusCodes.Status200OK)]
        public async Task<IResult> GetZoneDapr(CancellationToken cancellationToken)
        {
            var query = new GetZoneCount()
            {
            };
            var result = await _sender.Send(query, cancellationToken);

            return Results.Ok(result);
        }

    }
}
