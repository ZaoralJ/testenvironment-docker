using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.SqlEdge
{
    public class SqlEdgeContainerCleaner : IContainerCleaner<SqlEdgeContainer>
    {
        private const string CleanupCommand = "EXEC sp_MSforeachdb " +
            @"'IF DB_ID(''?'') > 4 BEGIN
                ALTER DATABASE [?] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                DROP DATABASE [?]
            END'";

        private readonly ILogger? _logger;

        public SqlEdgeContainerCleaner()
        {
        }

        public SqlEdgeContainerCleaner(ILogger logger) =>
            _logger = logger;

        public async Task CleanupAsync(SqlEdgeContainer container, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(container.GetConnectionString());
            await using var command = new SqlCommand(CleanupCommand, connection);
            try
            {
                await connection.OpenAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (SqlException e)
            {
                _logger?.LogInformation($"Cleanup issue: {e.Message}");
            }
        }

        public Task CleanupAsync(Container container, CancellationToken cancellationToken = default) => CleanupAsync((SqlEdgeContainer)container, cancellationToken);
    }
}
