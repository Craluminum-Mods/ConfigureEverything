using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration.ConfigClimbingSpeed;

public class ConfigClimbingSpeed : IModConfig
{
    public bool Enabled { get; set; }

    public readonly string Comment = "Ladder climbing speed";
    public double AscendingSpeed { get; set; } = 0.035;
    public double DescendingSpeed { get; set; } = 0.07;

    public ConfigClimbingSpeed(ICoreAPI api, ConfigClimbingSpeed previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            AscendingSpeed = previousConfig.AscendingSpeed;
            DescendingSpeed = previousConfig.DescendingSpeed;
        }
    }
}