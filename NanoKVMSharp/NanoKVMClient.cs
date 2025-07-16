using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace NanoKVMSharp
{
    public class NanoKVMClient
    {
        private readonly HttpClient _httpClient;

        public NanoKVMClient(string baseUrl)
        {
            if (!baseUrl.EndsWith('/'))
                baseUrl += "/";

            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public NanoKVMClient(string baseUrl, HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient = httpClient;
        }

        // --- Authentication Methods ---
        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = CryptoUtils.ObfuscatePassword(password)
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Login failed: {apiResponse?.Message}");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiResponse.Data?.Token);

            var loginData = apiResponse.Data!;

            _httpClient.DefaultRequestHeaders.Add("Cookie", "nano-kvm-token=" + loginData.Token);

            return loginData;
        }

        public async Task<IsPasswordUpdatedResponse> IsPasswordUpdatedAsync()
        {
            var response = await _httpClient.GetAsync("/api/auth/password");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IsPasswordUpdatedResponse>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to check password update status: {apiResponse?.Message}");

            return apiResponse.Data!;
        }

        public async Task ChangePasswordAsync(string username, string newPassword)
        {
            var request = new ChangePasswordRequest
            {
                Username = username,
                Password = CryptoUtils.ObfuscatePassword(newPassword)
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/password", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to change password: {apiResponse?.Message}");
        }

        public async Task SetGPIOAsync(GpioType type, int duration)
        {
            var request = new SetGpioRequest
            {
                Type = type.ToString().ToLower(),
                Duration = duration
            };

            var response = await _httpClient.PostAsJsonAsync("/api/vm/gpio", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to set GPIO: {apiResponse?.Message}");
        }

        public async Task<GetGpioResponse> GetGPIOStateAsync()
        {
            var response = await _httpClient.GetAsync("/api/vm/gpio");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetGpioResponse>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to get GPIO state: {apiResponse?.Message}");

            return apiResponse.Data!;
        }

        public async Task SetScreenSettingAsync(ScreenSettingType type, int value)
        {
            var request = new SetScreenRequest
            {
                Type = type,
                Value = value
            };

            var response = await _httpClient.PostAsJsonAsync("/api/vm/screen", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to set screen setting: {apiResponse?.Message}");
        }

        public async Task PasteTextAsync(string content)
        {
            var request = new PasteRequest
            {
                Content = content
            };

            var response = await _httpClient.PostAsJsonAsync("/api/hid/paste", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to paste text: {apiResponse?.Message}");
        }

        // --- Storage Methods ---
        public async Task<GetImagesResponse> GetImagesAsync()
        {
            var response = await _httpClient.GetAsync("/api/storage/image");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetImagesResponse>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to get images: {apiResponse?.Message}");

            return apiResponse.Data!;
        }

        public async Task MountImageAsync(string file, bool? cdRom = null)
        {
            var request = new MountImageRequest
            {
                File = file,
                CdRom = cdRom
            };

            var response = await _httpClient.PostAsJsonAsync("/api/storage/image/mount", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to mount image: {apiResponse?.Message}");
        }

        public async Task<GetMountedImageResponse> GetMountedImagesAsync()
        {
            var response = await _httpClient.GetAsync("/api/storage/image/mounted");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetMountedImageResponse>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to get mounted images: {apiResponse?.Message}");

            return apiResponse.Data!;
        }

        public async Task UnmountImageAsync()
        {
            var response = await _httpClient.DeleteAsync("/api/storage/image/mount");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to unmount image: {apiResponse?.Message}");
        }

        // --- Network Methods ---
        public async Task WakeOnLANAsync(string macAddress)
        {
            var request = new WakeOnLANRequest
            {
                Mac = macAddress
            };

            var response = await _httpClient.PostAsJsonAsync("/api/network/wol", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to send Wake-on-LAN: {apiResponse?.Message}");
        }

        public async Task<GetWifiResponse> GetWiFiStatusAsync()
        {
            var response = await _httpClient.GetAsync("/api/network/wifi");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetWifiResponse>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to get WiFi status: {apiResponse?.Message}");

            return apiResponse.Data!;
        }

        public async Task ConnectToWiFiAsync(string ssid, string password)
        {
            var request = new ConnectWifiRequest
            {
                SSID = ssid,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("/api/network/wifi", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to connect to WiFi: {apiResponse?.Message}");
        }

        // --- Application Methods ---
        public async Task<GetVersionResponse> GetVersionInfoAsync()
        {
            var response = await _httpClient.GetAsync("/api/application/version");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetVersionResponse>>();

            if (apiResponse?.Code != ApiResponseCode.SUCCESS)
                throw new Exception($"Failed to get version info: {apiResponse?.Message}");

            return apiResponse.Data!;
        }
    }
}
