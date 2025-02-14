namespace TransactionService.PL.Commands;

public interface ICommand
{
    string Id { get; }
}

public abstract record Command : ICommand
{
    public string Id { get; set; }
    public required AuditInfo AuditInfo { get; set; }
}