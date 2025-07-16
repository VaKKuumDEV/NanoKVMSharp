using NanoKVMSharp;
using System.Net.Http.Json;
using System.Net;
using RichardSzalay.MockHttp;

namespace Tests
{
    public class NanoKVMClientTests
    {
        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsException()
        {
            var handler = new MockHttpMessageHandler();
            handler
                .When("/api/auth/login")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new ApiResponse<LoginResponse>
                    {
                        Code = ApiResponseCode.INVALID_USERNAME_OR_PASSWORD,
                        Message = "Invalid credentials"
                    })
                });

            var client = new NanoKVMClient("http://localhost", handler.ToHttpClient());

            await Assert.ThrowsAsync<Exception>(() =>
                client.LoginAsync("admin", "wrong_password"));
        }

        [Fact]
        public async Task SetScreenSettingAsync_ValidRequest_SendsCorrectData()
        {
            HttpRequestMessage? lastRequest = null;

            var handler = new MockHttpMessageHandler();
            handler
                .When("/api/vm/screen")
                .Respond(req =>
                {
                    lastRequest = req;

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(new ApiResponse<object>
                        {
                            Code = ApiResponseCode.SUCCESS,
                            Message = "OK"
                        })
                    };
                });

            var client = new NanoKVMClient("http://localhost", handler.ToHttpClient());
            await client.SetScreenSettingAsync(ScreenSettingType.RESOLUTION, 1920);

            Assert.NotNull(lastRequest);
            Assert.NotNull(lastRequest.Content);

            var body = await lastRequest.Content.ReadFromJsonAsync<SetScreenRequest>();
            Assert.NotNull(body);

            Assert.Equal(ScreenSettingType.RESOLUTION, body.Type);
            Assert.Equal(1920, body.Value);
        }
    }
}
