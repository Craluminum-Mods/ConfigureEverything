using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace ConfigureEverything.Configuration.ConfigDurability;

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
        foreach (Block block in api.World.Blocks.Where(x => x.Durability != 0))
        {
            if (block?.Code != null && !Blocks.ContainsKey(block.Code.ToString()))
            {
                Blocks.Add(block.Code.ToString(), block.Durability);
            }
        }

        foreach (Item item in api.World.Items.Where(x => x.Durability != 0))
        {
            if (item?.Code != null && !Items.ContainsKey(item.Code.ToString()))
            {
                Items.Add(item.Code.ToString(), item.Durability);
            }
        }
    }

    public void ApplyPatches(ICoreAPI api)
    {
        if (Blocks.Any())
        {
            foreach (Block block in api.World.Blocks)
            {
                foreach ((string key, int value) in Blocks)
                {
                    if (block?.Code != null && WildcardUtil.Match(key, block.Code.ToString()))
                    {
                        block.Durability = value;
                        break;
                    }
                }
            }
        }

        if (Items.Any())
        {
            foreach (Item item in api.World.Items)
            {
                foreach ((string key, int value) in Items)
                {
                    if (item?.Code != null && WildcardUtil.Match(key, item.Code.ToString()))
                    {
                        item.Durability = value;
                        break;
                    }
                }
            }
        }
    }
}