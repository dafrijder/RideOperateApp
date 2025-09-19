namespace RideOperateApp
{
    public class Server
    {
        public string Name { get; set; } = "";
        public string Ip { get; set; } = "";
        public string Port { get; set; } = "";
        public string ApiKey { get; set; } = "";

        public string IpPort => $"{Ip}:{Port}";
    }
}
