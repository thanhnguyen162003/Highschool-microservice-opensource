namespace Application.OutboxProcess
{
	public class OutboxMessageProcessor : BackgroundService
	{
		protected override Task ExecuteAsync(CancellationToken
		  cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
