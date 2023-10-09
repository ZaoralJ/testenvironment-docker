using System;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.SqlEdge
{
    public class SqlEdgeContainerWaiter : BaseContainerWaiter<SqlEdgeContainer>
    {
        public SqlEdgeContainerWaiter()
        {
        }

        public SqlEdgeContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheckAsync(
            SqlEdgeContainer container,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(container.GetConnectionString());
            await using var command = new SqlCommand("SELECT @@VERSION", connection);

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);

            return true;
        }

        protected override bool IsRetryable(Exception exception) =>
            exception is InvalidOperationException
                or NotSupportedException
                or SqlException
                or SocketException;
    }
}