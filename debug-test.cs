using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        using var client = new HttpClient();
        try
        {
            var response = await client.GetStringAsync("http://localhost:5275/api/machines/debug/10.224.142.245");
            Console.WriteLine("Debug Response:");
            Console.WriteLine(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}