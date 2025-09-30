using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace MachineClient.WPF.Services;

/// <summary>
/// Service to get machine network information
/// </summary>
public interface IMachineInfoService
{
    string GetMacAddress();
    string GetIpAddress();
    string GetMachineName();
}

public class MachineInfoService : IMachineInfoService
{
    public string GetMacAddress()
    {
        try
        {
            // Get the first active network interface's MAC address
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(ni => 
                    ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                    !string.IsNullOrEmpty(ni.GetPhysicalAddress().ToString()));

            if (networkInterface != null)
            {
                var macBytes = networkInterface.GetPhysicalAddress().GetAddressBytes();
                return string.Join(":", macBytes.Select(b => b.ToString("X2")));
            }

            return "00:00:00:00:00:00";
        }
        catch
        {
            return "00:00:00:00:00:00";
        }
    }

    public string GetIpAddress()
    {
        try
        {
            // Get local IP address (not loopback)
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var localIP = host.AddressList
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && 
                                     !IPAddress.IsLoopback(ip));

            return localIP?.ToString() ?? "127.0.0.1";
        }
        catch
        {
            return "127.0.0.1";
        }
    }

    public string GetMachineName()
    {
        try
        {
            return Environment.MachineName;
        }
        catch
        {
            return "Unknown";
        }
    }
}