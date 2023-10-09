namespace TestEnvironment.Docker.Containers.SqlEdge
{
    public record SqlEdgeContainerParameters(string Name, string SAPassword)
        : ContainerParameters(Name, Constants.ContainerName)
    {
    }
}
