using System.ComponentModel.DataAnnotations;
using Application.Common.Models.TheoryModel;
using Application.Common.Ultils;
using Application.Features.TheoryFeature.Commands;
using Application.Features.TheoryFeature.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

[Route("api/v1/theory")]
[ApiController]
public class TheoryFileEndpoint(ISender sender, IMapper mapper) : ControllerBase
{
    private readonly IMapper _mapper = mapper;

    [HttpPost("{id}")]
    [Authorize("moderatorPolicy")]
    public async Task<IResult> FileTheoryUpload([FromForm] TheoryFileUploadRequestModel theoryFileUploadRequestModel,
        CancellationToken cancellationToken,[FromRoute, Required] Guid id)
    {
        var command = new TheoryFileCreateCommand()
        {
            TheoryFileUploadRequestModel = theoryFileUploadRequestModel,
            TheoryId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    [HttpGet("{id}")]
    public async Task<IResult> GetTheoryFiles(CancellationToken cancellationToken,[FromRoute, Required] Guid id)
    {
        var command = new TheoryFileQuery()
        {
            TheoryId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }

}
