using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ConfigureEverything.Configuration;

public class ConfigStackSizes : IModConfigWithDefaultValues
{
    public bool Enabled { get; set; }

    public bool FillWithDefaultValues { get; set; }

    public float Multiplier { get; set; } = 1.0f;

    public Dictionary<string, int> Blocks { get; set; } = new();

    public Dictionary<string, int> Items { get; set; } = new();

    public ConfigStackSizes(ICoreAPI api, ConfigStackSizes previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            Multiplier = previousConfig.Multiplier;
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
        Blocks.AddRange(GetDefaultStackSizesForBlocks(api));
        Items.AddRange(GetDefaultStackSizesForItems(api));
    }

    public void ApplyPatches(CollectibleObject obj)
    {
        if (Multiplier is not 0 and not 1)
        {
            if (obj.MaxStackSize * obj.MaxStackSize != obj.MaxStackSize)
            {
                obj.MaxStackSize = (int)(obj.MaxStackSize * Multiplier);
            }
            return;
        }

        switch (obj)
        {
            case Block when Blocks.Any():
                foreach ((string key, int value) in Blocks)
                {
                    if (obj.WildCardMatch(key))
                    {
                        obj.MaxStackSize = value;
                        break;
                    }
                }
                break;
            case Item when Items.Any():
                foreach ((string key, int value) in Items)
                {
                    if (obj.WildCardMatch(key))
                    {
                        obj.MaxStackSize = value;
                        break;
                    }
                }
                break;
        }
    }

    public Dictionary<string, int> GetDefaultStackSizesForBlocks(ICoreAPI api)
    {
        Dictionary<string, int> stackSizes = new();

        foreach (Block block in api.World.Blocks)
        {
            if (block is BlockMultiblock)
            {
                continue;
            }

            string code = block.Code?.ToString();
            if (!string.IsNullOrEmpty(code) && !stackSizes.ContainsKey(code))
            {
                stackSizes.Add(code, block.MaxStackSize);
            }
        }

        return stackSizes;
    }

    public Dictionary<string, int> GetDefaultStackSizesForItems(ICoreAPI api)
    {
        Dictionary<string, int> stackSizes = new();

        foreach (Item item in api.World.Items)
        {
            string code = item.Code?.ToString();
            if (!string.IsNullOrEmpty(code) && !stackSizes.ContainsKey(code))
            {
                stackSizes.Add(code, item.MaxStackSize);
            }
        }

        return stackSizes;
    }
}
