using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration.ConfigDurability;

public class ConfigDurability : IModConfigWithDefaultValues
{
    public bool Enabled { get; set; }
    public bool FillWithDefaultValues { get; set; }

    public Dictionary<string, int> Durability { get; set; } = new();

    public ConfigDurability(ICoreAPI api, ConfigDurability previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            Durability.AddRange(previousConfig.Durability);
        }

        if (api != null && FillWithDefaultValues)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (CollectibleObject obj in api.World.Collectibles.Where(x => x.Durability != 0))
        {
            if (!Durability.ContainsKey(obj.Code.ToString()))
            {
                Durability.Add(obj.Code.ToString(), obj.Durability);
            }
        }
    }
}