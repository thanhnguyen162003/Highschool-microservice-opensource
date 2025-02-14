using Microsoft.AspNetCore.SignalR;

namespace SignalR;
// <IHubService>
public class MyHub : Hub
{
    //private readonly IUnitOfWork _unitOfWork;
    //public MyHub(IUnitOfWork unitOfWork)
    //{
    //    _unitOfWork = unitOfWork;
    //}
    //[Authorize]
    //public async Task SendVoteAnswer(Guid answerId)
    //{
    //    var userClaims = Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
    //    var userId = Guid.Parse(userClaims);
    //    var result = await _voteService.AnswerVoting(userId, answerId);
    //    var votes = await unitOfWork.VoteRepository.GetVotesNumber(answerId, "answer");
    //    await Clients.All.SendAsync("VoteAnswer", result.Message, answerId ,votes);
    //}

    //[Authorize]
    //public async Task SendVoteDiscussion(Guid discussionId)
    //{
    //    var userClaims = Context.User?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
    //    var userId = Guid.Parse(userClaims);
    //    var result = await _voteService.DiscussionVoting(userId, discussionId);
    //    var votes = await _unitOfWork.VoteRepository.GetVotesNumber(discussionId, "discussion");
    //    await Clients.All.SendAsync("Voted", result.Message, votes);
    //}

    //[Authorize]
    //public async Task CreateUpVoteDiscussion(UpVoteCreateDiscussionRequestModel upvoteCreateRequestModel, ISender sender)
    //{
    //    var command = new CreateUpVoteDiscussionCommand()
    //    {
    //        UpVoteCreateRequestModel = upvoteCreateRequestModel,

    //    };
    //    var result = await sender.Send(command);
    //    var listChapter = await _unitOfWork.Upvotes.Get();
    //    await Clients.All.SendAsync("VoteAnswer", result.Message, upvoteCreateRequestModel.DiscussionId, listChapter.Count());
    //}
    //[Authorize]
    //public async Task CreateUpVoteComment(UpVoteCreateCommentRequestModel upvoteCreateRequestModel, ISender sender)
    //{
    //    var command = new CreateUpVoteCommentCommand()
    //    {
    //        UpVoteCreateRequestModel = upvoteCreateRequestModel,
    //    };
    //    var result = await sender.Send(command);
    //    var listChapter = await _unitOfWork.Upvotes.Get();
    //    await Clients.All.SendAsync("VoteAnswer", result.Message, upvoteCreateRequestModel.DiscussionId, listChapter.Count());
    //}

    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiverMessage: " +  $"{Context.ConnectionId} has joined");
        await base.OnConnectedAsync();
    }

}

