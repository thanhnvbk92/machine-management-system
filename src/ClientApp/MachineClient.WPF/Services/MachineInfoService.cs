using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Options;
using MachineClient.WPF.Models;

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
    private readonly ApiSettings _apiSettings;

    public MachineInfoService(IOptions<ApiSettings> apiSettings)
    {
        _apiSettings = apiSettings.Value;
    }

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
            // Get all local IP addresses (not loopback)
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var allIPs = host.AddressList
                .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && 
                            !IPAddress.IsLoopback(ip))
                .ToList();

            if (!allIPs.Any())
                return "127.0.0.1";

            // Ưu tiên IP theo dải được cấu hình (mặc định: 10.224.xxx.xxx)
            var preferredIP = allIPs.FirstOrDefault(ip => 
                ip.ToString().StartsWith(_apiSettings.PreferredIpPrefix));

            // Nếu có IP thuộc dải ưu tiên thì trả về, nếu không thì lấy IP đầu tiên
            return preferredIP?.ToString() ?? allIPs.First().ToString();
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