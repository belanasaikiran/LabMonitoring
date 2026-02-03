using System;
using System.Management;
using System.Net.NetworkInformation;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Runtime.Versioning;

namespace LabAgent
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("--- Enterprise Lab Agent: Phase 1 ---");

            using TcpClient client = new TcpClient("127.0.0.1", 5000);
            using NetworkStream stream = client.GetStream();


            string mac = GetMacAddress();
            Console.WriteLine($"[IDENTITY] MAC Address: {mac}");

            while (true)
            {
                double cpuLoad = GetCpuLoad();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Health Status: OK | CPU Load: {cpuLoad}%");

                bool firewallStatus = IsFirewallEnabled();
                string message = $"MAC:{mac}|CPU:{cpuLoad}%|FIREWALL:{(firewallStatus ? "SAFE" : "VULNERABLE")}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine($"[SENT] {message}");

                // Simulate a heartbeat interval
                //Thread.Sleep(2000);
                await Task.Delay(2000);
            }
        }

        // Get Active Mac Address
        static string GetMacAddress()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault() ?? "Not Found";

        }

        // CPU Metrics
        static double GetCpuLoad()
        {
            using var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                return Convert.ToDouble(obj["LoadPercentage"]);
            }
            return 0;
        }


        // Firewall enabled
        [SupportedOSPlatform("windows")]
        static bool IsFirewallEnabled()
        {
            using ServiceController sc = new ServiceController("MpsSvc");
            return sc.Status == ServiceControllerStatus.Running;
        }
    }
}