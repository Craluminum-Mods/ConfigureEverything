using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigSwimmingSpeed : IModConfigWithAutoFill
{
    [JsonProperty(Order = 1)]
    public bool Enabled { get; set; }

    [JsonProperty(Order = 2)]
    public bool AutoFill { get; set; }

    [JsonProperty(Order = 3)]
    public string Description => "Configure how fast rafts and boats can swim";

    [JsonProperty(Order = 4)]
    public Dictionary<string, float> SpeedMultiplier { get; set; } = new();

    public ConfigSwimmingSpeed(ICoreAPI api, ConfigSwimmingSpeed previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            AutoFill = previousConfig.AutoFill;

            SpeedMultiplier.AddRange(previousConfig.SpeedMultiplier);
        }

        if (api != null && AutoFill)
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