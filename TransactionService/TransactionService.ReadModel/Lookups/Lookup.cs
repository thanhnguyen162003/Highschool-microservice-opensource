namespace TransactionService.ReadModel;

public class Lookup
{
    public string Id { get; set; }
    public List<Ref> Data { get; set; }

    public Lookup()
    {
        Data = new List<Ref>();
    }
}