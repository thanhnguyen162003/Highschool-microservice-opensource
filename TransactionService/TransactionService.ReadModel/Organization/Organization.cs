namespace TransactionService.ReadModel;

public class Organization : ITypeAheadable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }

    public RefEx CovertToTypeaheadItem()
    {
        return new RefEx
        {
            Id = Starnet.Common.Utils.IdUtils.ToInt64(Id).ToString(),
            Val = Name
        };
    }

    public RefEx CovertToTypeaheadItem(string lng)
    {
        return CovertToTypeaheadItem();
    }
}