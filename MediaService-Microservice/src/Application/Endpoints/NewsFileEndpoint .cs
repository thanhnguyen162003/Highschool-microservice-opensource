using System.ComponentModel.DataAnnotations;
using Application.Common.Models.NewsModel;
using Application.Common.Ultils;
using Application.Features.NewsFeature.Commands;
using Application.Features.NewsFeature.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

[Route("api/v1/news")]
[ApiController]
public class NewsFileEndpoint(ISender sender, IMapper mapper) : ControllerBase
{
    private readonly IMapper _mapper = mapper;

    [HttpPost("{id}")]

    public async Task<IResult> FileNewsUpload([FromForm] NewsFileUploadRequestModel newFileUploadRequestModel,
        CancellationToken cancellationToken,[FromRoute, Required] Guid id)
    {
        var command = new NewsFileCreateCommand()
        {
            NewsFileUploadRequestModel = newFileUploadRequestModel,
            NewsId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    [HttpGet("{id}")]
    public async Task<IResult> GetNewsFiles(CancellationToken cancellationToken,[FromRoute, Required] Guid id)
    {
        var command = new NewsFileQuery()
        {
            NewsId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }

}
