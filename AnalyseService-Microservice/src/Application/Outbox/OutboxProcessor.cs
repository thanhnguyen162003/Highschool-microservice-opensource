//using System.Text.Json;
//using Dapper;
//using MassTransit;
//using Npgsql;

//namespace Orders.Api.Outbox;

//internal sealed class OutboxProcessor(NpgsqlDataSource dataSource, IPublishEndpoint publishEndpoint)
//{
//    private const int BatchSize = 10;

//    public async Task<int> Execute(CancellationToken cancellationToken = default)
//    {
//        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
//        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

//        var outboxMessages = (await connection.QueryAsync<OutboxMessage>(
//            """
//            SELECT *
//            FROM outbox_messages
//            WHERE processed_on_utc IS NULL
//            ORDER BY occurred_on_utc LIMIT @BatchSize
//            """,
//            new { BatchSize },
//            transaction: transaction)).AsList();

//        foreach (var outboxMessage in outboxMessages)
//        {
//            try
//            {
//                var messageType = Messaging.Contracts.AssemblyReference.Assembly.GetType(outboxMessage.Type)!;
//                var deserializedMessage = JsonSerializer.Deserialize(outboxMessage.Content, messageType)!;

//                await publishEndpoint.Publish(deserializedMessage, messageType, cancellationToken);

//                await connection.ExecuteAsync(
//                    """
//                    UPDATE outbox_messages
//                    SET processed_on_utc = @ProcessedOnUtc
//                    WHERE id = @Id
//                    """,
//                    new { ProcessedOnUtc = DateTime.UtcNow, outboxMessage.Id },
//                    transaction: transaction);
//            }
//            catch (Exception ex)
//            {
//                await connection.ExecuteAsync(
//                    """
//                    UPDATE outbox_messages
//                    SET processed_on_utc = @ProcessedOnUtc, error = @Error
//                    WHERE id = @Id
//                    """,
//                    new { ProcessedOnUtc = DateTime.UtcNow, Error = ex.ToString(), outboxMessage.Id },
//                    transaction: transaction);
//            }
//        }

//        await transaction.CommitAsync(cancellationToken);

//        return outboxMessages.Count;
//    }
//}
