using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.DaprModel.Document;
using Application.Common.Models.DaprModel.Flashcard;
using Application.Common.Models.DaprModel.User;
using Application.Common.Models.NewsModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.Services.CacheService.Interfaces;
using Dapr.Client;
using Domain.Entities.SqlEntites;
using Infrastructure.Data;
using MongoDB.Driver;

#pragma warning disable
namespace Application.Features.NewsFeature.Commands;

public record CreateNewsCommand : IRequest<ResponseModel>
{
    public NewsCreateRequestModel NewsCreateRequestModel;
}

public class CreateNewsCommandHandler(MediaDbContext dbContext, IMapper mapper,
    IClaimInterface claimInterface, ILogger<CreateNewsCommand> logger,
    DaprClient daprClient,
    IRedisDistributedCache redisCache)
    : IRequestHandler<CreateNewsCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingNews = dbContext.News
                .Find(name => name.NewName.Equals(request.NewsCreateRequestModel.NewName))
                .FirstOrDefault(cancellationToken);
            if (existingNews != null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NewExist);
            }
            // Initialize new news entry
            var userId = claimInterface.GetCurrentUserId;
            // gRPC Call to Fetch Authors
            var subjectGradeResponse = await daprClient.InvokeMethodAsync<UserResponseMediaDapr>(
                            HttpMethod.Get,
                            "user-sidecar",
                            $"api/v1/dapr/user-media?userId={userId.ToString()}"
                        );
            var authorLookup = new Author
            {
                AuthorId = Guid.Parse(subjectGradeResponse.UserId[0]),
                Avatar = subjectGradeResponse.Avatar[0],
                FullName = subjectGradeResponse.Username[0]
            };


            if (authorLookup is null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy người dùng");
            }
            // Check if news tag ID exists
            var tagExists = dbContext.NewsTags
                .Find(tag => tag.Id.Equals(request.NewsCreateRequestModel.NewsTagId))
                .FirstOrDefault(cancellationToken);
            if (tagExists == null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NotFound + "danh mục tin tức " + request.NewsCreateRequestModel.NewsTagId);
            }

            var news = mapper.Map<News>(request.NewsCreateRequestModel);

            if (tagExists.NewTagName.Equals("Tips"))
            {
                TipsErrorResponseModel tipsResponse = new TipsErrorResponseModel() 
                {
                };
                //grpc
                if (request.NewsCreateRequestModel.DocumentIds.Count() != 0)
                {
                    List<string> stringGuids = request.NewsCreateRequestModel.DocumentIds.Select(guid => guid.ToString()).Distinct().ToList();
                    DocumentTipsRequest documentRequest = new()
                    {
                    };
                    documentRequest.DocumentId.AddRange(stringGuids);
                    var documentResponse = await daprClient.InvokeMethodAsync<DocumentTipsResponseDapr>(
                            HttpMethod.Get,
                            "document-sidecar",
                            $"api/v1/dapr/document-tips?documentIds={string.Join("&documentIds=", documentRequest)}"
                        );
                    if (documentResponse.DocumentId.Count() > 0 && stringGuids.Count() == documentResponse.DocumentId.Count())
                    {
                        tipsResponse.Document = null;
                        tipsResponse.IsError = false;
                    }
                    else
                    {
                        var test = stringGuids.Except(documentResponse.DocumentId);
                        tipsResponse.Document = "Tài liệu này không tồn tại trong hệ thống " + string.Join(", ", test);
                        tipsResponse.IsError = true;
                    }
                }

                if (request.NewsCreateRequestModel.FlashcardIds.Count() != 0)
                {
                    List<string> ids = request.NewsCreateRequestModel.FlashcardIds.Select(guid => guid.ToString()).Distinct().ToList();
                    FlashcardTipsRequest flashcardRequest = new()
                    {
                    };
                    flashcardRequest.FlaschcardId.AddRange(ids);
                    var flashcardResponse = await daprClient.InvokeMethodAsync<FlashcardTipsResponseDapr>(
                            HttpMethod.Get,
                            "document-sidecar",
                            $"api/v1/dapr/flashcard-tips?flashcardIds={string.Join("&flashcardIds=", flashcardRequest)}"
                        );
                    if (flashcardResponse.FlaschcardId.Count() > 0 && ids.Count() == flashcardResponse.FlaschcardId.Count())
                    {
                        tipsResponse.Flashcard = null;
                        tipsResponse.IsError = false;
                    }
                    else
                    {
                        var test = ids.Except(flashcardResponse.FlaschcardId).ToList();
                        tipsResponse.Flashcard = "Thẻ ghi nhớ này không tồn tại trong hệ thống " + string.Join(", ", test);
                        tipsResponse.IsError = true;
                    }

                }

                
                //if (request.NewsCreateRequestModel.TheoryIds.Count() == 0)
                //{
                //    tipsResponse.Theory = "Lý thuyết này không tồn tại trong hệ thống ";
                //    List<string> ids = request.NewsCreateRequestModel.TheoryIds.Select(guid => guid.ToString()).ToList();
                //    TheoryTipsRequest theoryRequest = new()
                //    {
                //    };
                //    theoryRequest.TheoryId.AddRange(ids);
                //    var theoryResponse = await theoryServiceRpcClient.GetTheoryTipsAsync(theoryRequest);
                //    if (theoryResponse.TheoryId.Count() > 0)
                //    {
                //        tipsResponse.Theory = null;

                //    }
                //}

                if (tipsResponse.IsError == true)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.Error, tipsResponse);
                }
                //if (request.NewsCreateRequestModel.TheoryIds.IsNullOrEmpty() 
                //    && request.NewsCreateRequestModel.DocumentIds.IsNullOrEmpty()
                //    && request.NewsCreateRequestModel.FlashcardIds.IsNullOrEmpty())
                //{
                //    return new ResponseModel(HttpStatusCode.BadRequest, "Tips chưa có tài liệu đính kèm");
                //}
            }



            

            news.Id = new UuidV7().Value;
            news.Author = authorLookup;
            news.TodayView = 0;
            news.TotalView = 0;
            news.CreatedBy = userId;
            news.UpdatedBy = userId;
            news.Slug = SlugHelper.GenerateSlug(news.NewName, news.Id.ToString());
            news.CreatedAt = DateTime.UtcNow;
            news.UpdatedAt = DateTime.UtcNow;
            news.IsDeleted = false;
            news.Hot = false;

            //var result = await cloudinaryService.UploadAsync(request.NewsCreateRequestModel.Image);
            //if (result.Error != null)
            //{
            //    return new ResponseModel(HttpStatusCode.BadRequest, result.Error.Message);
            //}
            //NewsFile newsFile = new()
            //{
            //    NewsId = news.Id,
            //    UpdatedAt = DateTime.UtcNow,
            //    Id = ObjectId.GenerateNewId(),
            //    CreatedAt = DateTime.Now,
            //    FileType = result.ResourceType,
            //    File = result.Url.ToString(),
            //    FileExtention = result.Format,
            //    PublicId = result.PublicId
            //};
            //news.Image = newsFile.File;

            //news.Image = request.NewsCreateRequestModel.Image.

            await dbContext.News.InsertOneAsync(news, cancellationToken: cancellationToken);
            await ClearRelatedCache();

            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.CreateSuccess, news.Slug);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CreateFail);
        }
    }
    
    private async Task ClearRelatedCache()
    {
        var cachePatterns = new[] { "news:id:*", "news:tag:*", "news:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in redisCache.ScanAsync(pattern))
            {
                await redisCache.RemoveAsync(key);
            }
        }
    }
}
