using System;
using System.Net.Http;
using System.Threading.Tasks;

class TestHttpClient 
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Testing HttpClient connection to API...");
        
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5275/");
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        try 
        {
            Console.WriteLine("Making request to api/machines...");
            var response = await httpClient.GetAsync("api/machines");
            
            Console.WriteLine($"Status: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Content length: {content.Length}");
            Console.WriteLine($"First 200 chars: {content.Substring(0, Math.Min(200, content.Length))}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}