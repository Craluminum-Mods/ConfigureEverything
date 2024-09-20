using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration;

public class ConfigToolTier : IModConfigWithDefaultValues
{
    public bool Enabled { get; set; }
    public bool FillWithDefaultValues { get; set; }

    public Dictionary<string, int> Blocks { get; set; } = new();

    public Dictionary<string, int> Items { get; set; } = new();

    public ConfigToolTier(ICoreAPI api, ConfigToolTier previousConfig = null)
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
            string code = obj?.Code?.CodeWithoutDefaultDomain();
            if (code != null && obj.ToolTier != 0 && !Blocks.ContainsKey(code))
            {
                Blocks.Add(code, obj.ToolTier);
            }
        }

        foreach (Item obj in api.World.Items)
        {
            string code = obj?.Code?.CodeWithoutDefaultDomain();
            if (code != null && obj.ToolTier != 0 && !Items.ContainsKey(code))
            {
                Items.Add(code, obj.ToolTier);
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
                        obj.ToolTier = value;
                        break;
                    }
                }
                break;
            case Item when Items.Any():
                foreach ((string key, int value) in Items)
                {
                    if (obj.WildCardMatch(key))
                    {
                        obj.ToolTier = value;
                        break;
                    }
                }
                break;
        }
    }
}