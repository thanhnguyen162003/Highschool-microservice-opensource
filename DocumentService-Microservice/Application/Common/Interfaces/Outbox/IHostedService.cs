namespace Application.Common.Interfaces.Outbox
{
	public interface IHostedService
	{
		Task StartAsync(CancellationToken cancellationToken);
		Task StopAsync(CancellationToken cancellationToken);
	}

}
