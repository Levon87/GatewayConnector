namespace DataTransitGateway;

public class OcelotConfiguration
{
    public List<RouteConfig> Routes { get; set; }
}

public class RouteConfig
{
    public string DownstreamPathTemplate { get; set; }
    public string DownstreamScheme { get; set; }
    public List<DownstreamHostAndPort> DownstreamHostAndPorts { get; set; }
    public string UpstreamPathTemplate { get; set; }
    public List<string> UpstreamHttpMethod { get; set; }
}

public class DownstreamHostAndPort
{
    public string Host { get; set; }
    public int Port { get; set; }
}
