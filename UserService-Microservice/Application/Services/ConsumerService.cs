using Confluent.Kafka;

namespace Application.Services;

public abstract class ConsumerService : BackgroundService
{
	private readonly IConsumer<string, string> _consumer;
	private readonly ILogger<ConsumerService> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly string _topicName;

	public ConsumerService(IConfiguration configuration, ILogger<ConsumerService> logger,
		IServiceProvider serviceProvider, string topicName)
	{
		_logger = logger;
		_topicName = topicName;
		_serviceProvider = serviceProvider;

		var consumerConfig = new ConsumerConfig
		{
			BootstrapServers = configuration["Kafka:BootstrapServers"],
			SaslUsername = configuration["Kafka:SaslUsername"],
			SaslPassword = configuration["Kafka:SaslPassword"],
			SecurityProtocol = SecurityProtocol.SaslPlaintext,
			SaslMechanism = SaslMechanism.Plain,
			GroupId = "user_consumer_group",
			AutoOffsetReset = AutoOffsetReset.Earliest,
			EnableAutoCommit = true,
			// SessionTimeoutMs = 6000,
			// MaxPollIntervalMs = 1000
		};
		_consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
	}
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_consumer.Subscribe(_topicName);

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(3));

				if (consumeResult != null)
				{
					var message = consumeResult.Message.Value;
					_logger.LogInformation($"Received update: {message}");
					// Process the message asynchronously
					await HandleMessageAsync(message, _serviceProvider);
				} else
				{
					_logger.LogInformation("No messages available.");
				}
			} catch (ConsumeException e)
			{
				_logger.LogError($"Consume error: {e.Error.Reason}");
			} catch (OperationCanceledException)
			{
				_logger.LogInformation("Kafka consumption was canceled.");
			} catch (Exception ex)
			{
				_logger.LogError($"Error processing Kafka message: {ex.Message}");
			}
			await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
		}
		_consumer.Unsubscribe();
		_consumer.Close();
	}
	protected abstract Task HandleMessageAsync(string message, IServiceProvider serviceProvider);
}
