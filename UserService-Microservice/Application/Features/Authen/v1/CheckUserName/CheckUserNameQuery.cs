namespace Application.Features.Authen.v1.CheckUserName
{
    public class CheckUserNameQuery : IRequest<bool>
    {
        public string UserName { get; init; } = null!;
    }
}
