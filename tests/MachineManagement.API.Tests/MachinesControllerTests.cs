using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MachineManagement.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MachineManagement.API.Tests
{
    public class MachinesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public MachinesControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterMachine_NewMachine_ReturnsSuccessAndIsNew()
        {
            var request = new MachineRegistrationRequest
            {
                IP = "192.168.100.100",
                MacAddress = "AA:BB:CC:DD:EE:FF",
                MachineName = "TestMachine",
                AppVersion = "1.0.0"
            };

            var response = await _client.PostAsJsonAsync("/api/machines/register", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<MachineRegistrationResponse>();

            Assert.NotNull(result);
            Assert.True(result!.IsSuccess);
            Assert.True(result.IsNewMachine);
            Assert.NotNull(result.MachineInfo);
            Assert.Equal("TestMachine", result.MachineInfo!.Name);
            Assert.Equal("AA:BB:CC:DD:EE:FF", result.MachineInfo.MacAddress);
            Assert.Equal("192.168.100.100", result.MachineInfo.IP);
        }
    }
}
