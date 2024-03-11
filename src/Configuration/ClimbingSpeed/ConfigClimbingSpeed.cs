using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration.ConfigClimbingSpeed;

public class ConfigClimbingSpeed : IModConfig
{
    public bool Enabled { get; set; }

    public readonly string Comment = "Ladder climbing speed";
    public readonly float DefaultUpSpeed = 0.07f;
    public readonly float DefaultDownSpeed = 0.035f;
    public float UpSpeed { get; set; } = 0.07f;
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