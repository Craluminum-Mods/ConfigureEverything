using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration;

public class ConfigClimbingSpeed : IModConfig
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public string Comment = "Ladder climbing speed";

    [JsonProperty(Order = 3)]
    public float DefaultUpSpeed = 0.07f;

    [JsonProperty(Order = 4)]
    public float DefaultDownSpeed = 0.035f;

    [JsonProperty(Order = 5)]
    public float UpSpeed { get; set; } = 0.07f;

    [JsonProperty(Order = 6)]
    public float DownSpeed { get; set; } = 0.035f;

    public ConfigClimbingSpeed(ICoreAPI api, ConfigClimbingSpeed previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;

            UpSpeed = previousConfig.UpSpeed;
            DownSpeed = previousConfig.DownSpeed;
        }
    }
}