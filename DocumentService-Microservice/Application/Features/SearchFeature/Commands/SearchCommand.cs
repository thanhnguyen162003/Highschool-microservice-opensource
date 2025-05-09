using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Services.SearchService;
using Domain.Enums;
using System.Net;

namespace Application.Features.SearchFeature.commands
{
    public class SearchCommand : IRequest<ResponseModel>
    {
        public string IndexName { get; set; } = null!;
    }

    public class SearchCommandHandler(IAlgoliaService algoliaService) : IRequestHandler<SearchCommand, ResponseModel>
    {
        private readonly IAlgoliaService _algoliaService = algoliaService;

        public async Task<ResponseModel> Handle(SearchCommand request, CancellationToken cancellationToken)
        {
            var result = false;
            if (request.IndexName.Equals("course"))
            {
                result = await _algoliaService.MigrateDataCourse();
            } else if (request.IndexName.Equals("search"))
            {
                result = await _algoliaService.MigrateData();
            }

            return result ? new ResponseModel { Status = HttpStatusCode.OK, Message = "Index created successfully" } : new ResponseModel { Status = HttpStatusCode.BadRequest, Message = "Failed to create index" };
        }


    }
}
