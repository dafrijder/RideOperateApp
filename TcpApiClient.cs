using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RideOperateApp
{
    public static class TcpApiClient
    {
        public static async Task<string[]> GetPanelsAsync(Server server)
        {
            try
            {
                using TcpClient client = new(server.Ip, int.Parse(server.Port));
                using NetworkStream stream = client.GetStream();
                using StreamWriter writer = new(stream) { AutoFlush = true };
                using StreamReader reader = new(stream);

                await writer.WriteLineAsync(server.ApiKey);
                await writer.WriteLineAsync("GET_PANELS");

                var panels = new List<string>();
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                    panels.Add(line);

                return panels.ToArray();
            }
            catch (Exception ex)
            {
                return new string[] { "ERROR: " + ex.Message };
            }
        }

        public static async Task<Dictionary<string, string>> GetPanelActionsAsync(Server server, string panelName)
        {
            var dict = new Dictionary<string, string>();
            try
            {
                using TcpClient client = new(server.Ip, int.Parse(server.Port));
                using NetworkStream stream = client.GetStream();
                using StreamWriter writer = new(stream) { AutoFlush = true };
                using StreamReader reader = new(stream);

                await writer.WriteLineAsync(server.ApiKey);
                await writer.WriteLineAsync("GET_PANEL:" + panelName);

                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line.StartsWith("ERROR")) break;
                    var parts = line.Split(':', 2);
                    if (parts.Length == 2)
                        dict[parts[0]] = parts[1];
                }
            }
            catch { }

            return dict;
        }

        public static async Task<bool> ExecuteActionAsync(Server server, string panelName, string action)
        {
            try
            {
                using TcpClient client = new(server.Ip, int.Parse(server.Port));
                using NetworkStream stream = client.GetStream();
                using StreamWriter writer = new(stream) { AutoFlush = true };
                using StreamReader reader = new(stream);

                await writer.WriteLineAsync(server.ApiKey);
                await writer.WriteLineAsync($"EXECUTE:{panelName}:{action}");

                string response = await reader.ReadLineAsync();
                return response?.Trim() == "OK";
            }
            catch { return false; }
        }
    }
}
