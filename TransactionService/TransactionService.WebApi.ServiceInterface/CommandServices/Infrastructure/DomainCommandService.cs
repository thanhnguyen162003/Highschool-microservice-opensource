namespace TransactionService.WebApi.ServiceInterface;

public class DomainCommandService : Service
{
    protected IMessageBus Bus { get; set; }
    protected ITimeProvider TimeProvider { get; set; }

    public DomainCommandService(IMessageBus bus, ITimeProvider timeProvider)
    {
        Bus = bus;
        TimeProvider = timeProvider;
    }

    protected async Task<ResponseStatus> TryAutoMapAndProcessRequest<T>(object command)
    {
        T cmd = command.ConvertTo<T>();
        AddAuditInfoToCommand(cmd as PL.Commands.Command);
        await Bus.Send(cmd).ConfigureAwait(false);
        return new ResponseStatus();
    }

    protected async Task<ResponseStatus> TryProcessRequest(PL.Commands.Command cmd)
    {
        AddAuditInfoToCommand(cmd);
        await Bus.Send(cmd).ConfigureAwait(false);
        return new ResponseStatus();
    }

    protected void AddAuditInfoToCommand(PL.Commands.Command cmd)
    {
        cmd.AuditInfo = GetAuditInfo();
    }

    protected AuditInfo GetAuditInfo()
    {
        var ses = Request.GetSession();
        return new AuditInfo { Time = TimeProvider.GetUtcTime(), Issuer = $"{Consts.IdPrefixes.User}{ses.Id}" };
    }
}