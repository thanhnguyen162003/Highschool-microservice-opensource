using System.Security.Claims;

namespace TransactionService.Api.ServiceInterface;

[ApiController]
public class CommandControllerBase : ControllerBase
{
    protected readonly ITimeProvider TimeProvider;
    readonly IMapper Mapper;
    readonly IMessageSession Bus;

    public CommandControllerBase(IMessageSession bus, ITimeProvider timeProvider, IMapper mapper)
    {
        Bus = bus;
        TimeProvider = timeProvider;
        Mapper = mapper;
    }

    protected virtual async Task MapAndProcessRequest<T>(object command)
    {
        T cmd = Mapper.Map<T>(command);
        AddAuditInfoToCommand(cmd as PL.Commands.Command);
        await Bus.Send(cmd);
    }

    protected virtual async Task ProcessRequest(object cmd)
    {
        AddAuditInfoToCommand(cmd as PL.Commands.Command);
        await Bus.Send(cmd);
    }

    void AddAuditInfoToCommand(PL.Commands.Command cmd)
    {
        string? id = GetUserId();
        var email = User.Claims.Where(c => c.Type == ClaimTypes.Email)
            .Select(c => c.Value).SingleOrDefault();

        cmd.AuditInfo = new AuditInfo
        {
            Time = TimeProvider.GetUtcTime(),
            Issuer = id
        };
    }


    protected string? GetUserId()
    {
        var userId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault();
        return userId == null ? null : $"{Consts.IdPrefixes.User}{userId}";
    }
}