using NanoKVMSharp;
using System.Text.Json;

namespace Tests
{
    public class ModelsTests
    {
        [Fact]
        public void DeserializeGetInfoResponse_ValidJson_ReturnsCorrectObject()
        {
            string json = @"
            {
                ""code"": 0,
                ""msg"": ""OK"",
                ""data"": {
                    ""ips"": [
                        { ""name"": ""eth0"", ""addr"": ""192.168.1.1"", ""version"": ""ipv4"", ""type"": ""lan"" }
                    ],
                    ""mdns"": ""nanokvm.local"",
                    ""image"": ""v1.0.0"",
                    ""application"": ""NanoKVM"",
                    ""deviceKey"": ""abc123""
                }
            }";

            var response = JsonSerializer.Deserialize<ApiResponse<GetInfoResponse>>(json);
            Assert.NotNull(response?.Data);
            Assert.Equal("nanokvm.local", response.Data.MDNS);
            Assert.Equal("abc123", response.Data.DeviceKey);
        }

        [Fact]
        public void DeserializeGetSwapStateResponse_NullEnabled_ReturnsNull()
        {
            string json = @"
            {
                ""code"": 0,
                ""msg"": ""OK"",
                ""data"": {
                    ""enabled"": null
                }
            }";

            var response = JsonSerializer.Deserialize<ApiResponse<GetSwapStateResponse>>(json);
            Assert.Null(response?.Data?.Enabled);
        }

        [Fact]
        public void DeserializeIsPasswordUpdatedResponse_AliasWorks()
        {
            string json = @"
            {
                ""code"": 0,
                ""msg"": ""OK"",
                ""data"": {
                    ""isUpdated"": true
                }
            }";

            var response = JsonSerializer.Deserialize<ApiResponse<IsPasswordUpdatedResponse>>(json);
            Assert.True(response?.Data?.IsUpdated);
        }

        [Fact]
        public void DeserializeApiResponse_InvalidJson_ThrowsException()
        {
            string json = "invalid_json";
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(json));
        }
    }
}
