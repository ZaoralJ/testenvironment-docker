using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.SqlEdge
{
    public sealed class SqlEdgeContainer : Container
    {
        private readonly SqlEdgeContainerParameters _parameters;

        public SqlEdgeContainer(SqlEdgeContainerParameters containerParameters)
            : base(containerParameters) =>
            _parameters = containerParameters;

        public SqlEdgeContainer(SqlEdgeContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient) =>
            _parameters = containerParameters;

        public SqlEdgeContainer(SqlEdgeContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger) =>
            _parameters = containerParameters;

        public SqlEdgeContainer(SqlEdgeContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger) =>
            _parameters = containerParameters;

        public string GetConnectionString() =>
            $"Data Source={(IsDockerInDocker ? IPAddress : IP.Loopback.ToString())}, {(IsDockerInDocker ? 1433 : Ports![1433])};User ID=sa;Password={_parameters.SAPassword};Encrypt=True;TrustServerCertificate=True;";
    }
}
