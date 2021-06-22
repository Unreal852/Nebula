namespace Nebula.Net.Server
{
    public class NetServerSettings
    {
        public static NetServerSettings Default { get; } = new(10, 9080, 100, 1500, 10000, true, "");

        public NetServerSettings()
        {
        }

        public NetServerSettings(int maxClients, int serverPort, int badPacketsLimit, int mediaChangeDelay, int upNpTimeOut, bool useUpNp, string key)
        {
            MaxClients = maxClients;
            ServerPort = serverPort;
            BadPacketsLimit = badPacketsLimit;
            MediaChangeDelay = mediaChangeDelay;
            UpNpTimeOut = upNpTimeOut;
            UseUpNp = useUpNp;
            Key = key;
        }

        public int    MaxClients       { get; init; }
        public int    ServerPort       { get; init; }
        public int    BadPacketsLimit  { get; init; }
        public int    MediaChangeDelay { get; init; }
        public int    UpNpTimeOut      { get; init; }
        public bool   UseUpNp          { get; init; }
        public string Key              { get; init; }
    }
}