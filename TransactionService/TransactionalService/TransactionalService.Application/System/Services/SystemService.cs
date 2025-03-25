namespace TransactionalService.Application
{
    public class SystemService : ISystemService, ITransient
    {
        public string GetDescription()
        {
            return "Testing new API";
        }
    }
}
