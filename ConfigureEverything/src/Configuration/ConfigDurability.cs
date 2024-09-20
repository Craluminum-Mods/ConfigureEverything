using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigDurability : IModConfigWithDefaultValues
{
    public bool Enabled { get; set; }
    public bool FillWithDefaultValues { get; set; }

    public Dictionary<string, int> Blocks { get; set; } = new();
    public Dictionary<string, int> Items { get; set; } = new();

    public ConfigDurability(ICoreAPI api, ConfigDurability previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            Blocks.AddRange(previousConfig.Blocks);
            Items.AddRange(previousConfig.Items);
        }

        if (api != null && FillWithDefaultValues)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach (Block obj in api.World.Blocks)
        {
            if (obj?.Code != null && obj.Durability != 0 && !Blocks.ContainsKey(obj.Code.ToString()))
            {
                Blocks.Add(obj.Code.ToString(), obj.Durability);
            }
        }

        foreach (Item obj in api.World.Items)
        {
            if (obj?.Code != null && obj.Durability != 0 && !Items.ContainsKey(obj.Code.ToString()))
            {
                Items.Add(obj.Code.ToString(), obj.Durability);
            }
        }
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, int value) in Blocks)
                {
                    if (obj.WildCardMatch(key))
                    {
                        obj.Durability = value;
                        break;
                    }
                }
                break;
            case Item when Items.Any():
                foreach ((string key, int value) in Items)
                {
                    if (obj.WildCardMatch(key))
                    {
                        obj.Durability = value;
                        break;
                    }
                }
                break;
        }
    }
}