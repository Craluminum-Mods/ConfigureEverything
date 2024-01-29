using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace ConfigureEverything.Configuration.ConfigSwimmingSpeed;

public class ConfigSwimmingSpeed : IModConfig
{
    public bool Enabled { get; set; }

    public Dictionary<string, float> SpeedMultiplier { get; set; } = new();

    public ConfigSwimmingSpeed(ICoreAPI api, ConfigSwimmingSpeed previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;

            foreach ((string key, float value) in previousConfig.SpeedMultiplier)
            {
                if (!SpeedMultiplier.ContainsKey(key))
                {
                    SpeedMultiplier.Add(key, value);
                }
            }
        }

        if (api != null)
        {
            FillDefault(api);
        }
    }

    private void FillDefault(ICoreAPI api)
    {
        foreach (EntityProperties key in api.World.EntityTypes.Where(x => x.IsBoat()).ToList())
        {
            if (!SpeedMultiplier.ContainsKey(key.Code.ToString()))
            {
                SpeedMultiplier.Add(key.Code.ToString(), 1);
            }
        }
    }
}