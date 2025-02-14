using Application.Features.KetFeatures.Command;
using Application.Features.KetFeatures.Query;
using Domain.Models.Common;
using Domain.Models.KetModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Controllers
{
    [Route("api/v1/kets")]
    [ApiController]
    public class KetController : Controller
    {
        private readonly ISender _sender;

        public KetController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> CreateKet([FromBody] CreateKetCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPatch("{ketId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> UpdateKet([FromRoute] Guid ketId, [FromBody] UpdateKetCommand command, CancellationToken cancellationToken = default)
        {
            command.KetId = ketId;
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{ketId}")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> DeleteKet([FromRoute] Guid ketId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new DeleteKetCommand { KetId = ketId }, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<KetResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetKets([FromQuery] GetKetQuery getKetQuery, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(getKetQuery, cancellationToken);

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(result.Metadata));

            return Ok(result);
        }

        [HttpGet("categories")]
        [ProducesResponseType(typeof(IEnumerable<KetResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetKetCategories([FromQuery] GetKetCategoryQuery getKetCategoryQuery, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(getKetCategoryQuery, cancellationToken);

            return Ok(result);
        }
    }
}
