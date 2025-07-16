using System.Runtime;
using System.Text.Json.Serialization;

namespace NanoKVMSharp
{
    public enum ApiResponseCode
    {
        SUCCESS = 0,
        FAILURE = -1,
        INVALID_USERNAME_OR_PASSWORD = -2
    }

    public enum HidMode
    {
        NORMAL,
        HID_ONLY
    }

    public enum GpioType
    {
        RESET,
        POWER
    }

    public enum ScreenSettingType
    {
        RESOLUTION,
        FPS,
        QUALITY
    }

    public enum RunScriptType
    {
        FOREGROUND,
        BACKGROUND
    }

    public enum VirtualDevice
    {
        NETWORK,
        DISK
    }

    public enum TailscaleState
    {
        NOT_INSTALLED,
        NOT_RUNNING,
        NOT_LOGIN,
        STOPPED,
        RUNNING
    }

    public enum TailscaleAction
    {
        INSTALL,
        UNINSTALL,
        UP,
        DOWN,
        LOGIN,
        LOGOUT,
        START,
        STOP,
        RESTART
    }

    public enum DownloadStatus
    {
        IDLE,
        IN_PROGRESS
    }

    public enum HWVersion
    {
        ALPHA,
        BETA,
        PCIE,
        UNKNOWN
    }

    public class ApiResponse<T>
    {
        [JsonPropertyName("code")]
        public ApiResponseCode Code { get; set; }

        [JsonPropertyName("msg")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class LoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }

    public class GetAccountResponse
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }

    public class ChangePasswordRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class IsPasswordUpdatedResponse
    {
        [JsonPropertyName("isUpdated")]
        public bool IsUpdated { get; set; }
    }

    public class IPInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("addr")]
        public string Address { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class GetInfoResponse
    {
        [JsonPropertyName("ips")]
        public List<IPInfo> IPs { get; set; }

        [JsonPropertyName("mdns")]
        public string MDNS { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("application")]
        public string Application { get; set; }

        [JsonPropertyName("deviceKey")]
        public string DeviceKey { get; set; }
    }

    public class SetGpioRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

        [JsonPropertyName("duration")]
        public int Duration { get; set; } // В миллисекундах
    }

    public class GetGpioResponse
    {
        [JsonPropertyName("pwr")]
        public bool PowerLED { get; set; } // Состояние питания

        [JsonPropertyName("hdd")]
        public bool HDDLED { get; set; } // Только для Alpha-аппаратуры
    }
    public class SetScreenRequest
    {
        [JsonPropertyName("type")]
        public ScreenSettingType Type { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    public class GetVirtualDeviceResponse
    {
        [JsonPropertyName("network")]
        public bool Network { get; set; }

        [JsonPropertyName("disk")]
        public bool Disk { get; set; }
    }

    public class UpdateVirtualDeviceRequest
    {
        [JsonPropertyName("device")]
        public VirtualDevice Device { get; set; }
    }

    public class UpdateVirtualDeviceResponse
    {
        [JsonPropertyName("on")]
        public bool On { get; set; }
    }

    public class GetMemoryLimitResponse
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; } // В МБ
    }

    public class SetMemoryLimitRequest
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; } // В МБ
    }

    public class GetOLEDDisplayResponse
    {
        [JsonPropertyName("exist")]
        public bool Exists { get; set; }

        [JsonPropertyName("sleep")]
        public int SleepTimeout { get; set; } // В секундах
    }

    public class SetOLEDDisplayRequest
    {
        [JsonPropertyName("sleep")]
        public int SleepTimeout { get; set; } // В секундах
    }

    public class GetSSHStateResponse
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
    }

    public class GetSwapStateResponse
    {
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; }
    }

    public class GetMDNSStateResponse
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
    }

    public class GetHidModeResponse
    {
        [JsonPropertyName("mode")]
        public HidMode Mode { get; set; }
    }

    public class SetHidModeRequest
    {
        [JsonPropertyName("mode")]
        public HidMode Mode { get; set; }
    }

    public class PasteRequest
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class GetImagesResponse
    {
        [JsonPropertyName("files")]
        public List<string> Files { get; set; }
    }

    public class MountImageRequest
    {
        [JsonPropertyName("file")]
        public string File { get; set; }

        [JsonPropertyName("cdrom")]
        public bool? CdRom { get; set; }
    }

    public class GetMountedImageResponse
    {
        [JsonPropertyName("file")]
        public string File { get; set; } // Путь к смонтированному файлу
    }

    public class GetCdRomResponse
    {
        [JsonPropertyName("cdrom")]
        public int CdRom { get; set; }
    }

    public class WakeOnLANRequest
    {
        [JsonPropertyName("mac")]
        public string Mac { get; set; }
    }

    public class GetMacResponse
    {
        [JsonPropertyName("macs")]
        public List<string> Macs { get; set; }
    }

    public class DeleteMacRequest
    {
        [JsonPropertyName("mac")]
        public string Mac { get; set; }
    }

    public class GetWifiResponse
    {
        [JsonPropertyName("supported")]
        public bool Supported { get; set; }

        [JsonPropertyName("connected")]
        public bool Connected { get; set; }
    }

    public class ConnectWifiRequest
    {
        [JsonPropertyName("ssid")]
        public string SSID { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class GetTailscaleStatusResponse
    {
        [JsonPropertyName("state")]
        public TailscaleState State { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("account")]
        public string Account { get; set; }
    }

    public class GetVersionResponse
    {
        [JsonPropertyName("current")]
        public string Current { get; set; }

        [JsonPropertyName("latest")]
        public string Latest { get; set; }
    }

    public class GetPreviewResponse
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
    }

    public class SetPreviewRequest
    {
        [JsonPropertyName("enable")]
        public bool Enable { get; set; }
    }

    public class ImageEnabledResponse
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
    }

    public class StatusImageResponse
    {
        [JsonPropertyName("status")]
        public DownloadStatus Status { get; set; }

        [JsonPropertyName("file")]
        public string File { get; set; }

        [JsonPropertyName("percentage")]
        public string Percentage { get; set; }
    }

    public class DownloadImageRequest
    {
        [JsonPropertyName("file")]
        public string File { get; set; } // URL для загрузки
    }


}
