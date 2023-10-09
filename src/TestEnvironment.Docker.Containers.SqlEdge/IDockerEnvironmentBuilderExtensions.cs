using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.SqlEdge
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static SqlEdgeContainerParameters DefaultParameters => new("mssql", "password")
        {
            ImageName = Constants.ContainerName,
            ContainerCleaner = new SqlEdgeContainerCleaner(),
            ContainerWaiter = new SqlEdgeContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddSqlEdgeContainer(
            this IDockerEnvironmentBuilder builder,
            Func<SqlEdgeContainerParameters, SqlEdgeContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new SqlEdgeContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddSqlEdgeContainer(
            this IDockerEnvironmentBuilder builder,
            Func<SqlEdgeContainerParameters, IDockerClient, ILogger?, SqlEdgeContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new SqlEdgeContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        private static SqlEdgeContainerParameters FixEnvironmentVariables(SqlEdgeContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["ACCEPT_EULA"] = "Y",
                    ["SA_PASSWORD"] = p.SAPassword
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static SqlEdgeContainerParameters GetDefaultParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                not null => DefaultParameters with
                {
                    ContainerWaiter = new SqlEdgeContainerWaiter(builder.Logger),
                    ContainerCleaner = new SqlEdgeContainerCleaner(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}
