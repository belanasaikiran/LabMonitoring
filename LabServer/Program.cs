using LabServer;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class LabServerC{
    static async Task Main(){
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("[SERVER] Lab Monitoring Server Started on port 5000....");

        while(true){
            TcpClient client = await server.AcceptTcpClientAsync();

            // handling each agent in a separate Task (Thread-Like)
            // so the server doesn't block
            _ = Task.Run(() => HandleAgent(client));
        }
    }

    static async Task HandleAgent(TcpClient client)
    {
        using (client)
        {
            Console.WriteLine($"[CONNECTED] Agent from {client.Client.RemoteEndPoint}");
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            // Ensure database is created
            using var db = new LabContext();
            db.Database.EnsureCreated();

            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Parse the data (MAC:XX:XX|CPU:15%)
                    var parts = data.Split('|');
                    var mac = parts[0].Replace("MAC:", "");
                    var cpu = parts[1].Replace("CPU:", "");
                    var firewall = parts[2].Replace("FIREWALL:", "");

                    // Save to Database
                    db.Logs.Add(new MachineLog
                    {
                        MacAddress = mac,
                        CpuLoad = cpu,
                        FireWallStatus = firewall,
                        TimeStamp = DateTime.Now
                    });
                    await db.SaveChangesAsync();

                    Console.WriteLine($"[DATA RECEIVED] {data}");
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine($"[ERROR] {e.Message}");
            }

        }
    }
}
