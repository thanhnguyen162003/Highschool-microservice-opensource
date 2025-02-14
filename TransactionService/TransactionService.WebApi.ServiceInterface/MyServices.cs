namespace TransactionService.WebApi.ServiceInterface;

public class HelloServices : Service
{
    public object Any(Hello request)
    {
        return new HelloResponse { Result = $"Hello, {request.Name}!" };
    }
}