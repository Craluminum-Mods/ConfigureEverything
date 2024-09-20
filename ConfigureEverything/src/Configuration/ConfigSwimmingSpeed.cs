using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigSwimmingSpeed : IModConfigWithDefaultValues
{
    public bool Enabled { get; set; }

    public bool FillWithDefaultValues { get; set; }

    public Dictionary<string, float> SpeedMultiplier { get; set; } = new();

    public ConfigSwimmingSpeed(ICoreAPI api, ConfigSwimmingSpeed previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            SpeedMultiplier.AddRange(previousConfig.SpeedMultiplier);
        }

        if (api != null && FillWithDefaultValues)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (EntityProperties key in api.World.EntityTypes.Where(x => x.IsBoat()))
        {
            if (!SpeedMultiplier.ContainsKey(key.Code.ToString()))
            {
                SpeedMultiplier.Add(key.Code.ToString(), 1);
            }
        }
    }
}