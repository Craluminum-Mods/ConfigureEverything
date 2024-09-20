using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ConfigureEverything.Configuration.ConfigStackSizes;

public class ConfigStackSizes : IModConfigWithDefaultValues
{
    public bool Enabled { get; set; }
    public bool FillWithDefaultValues { get; set; }

    public float StackSizeMultiplier { get; set; } = 1.0f;
    public Dictionary<string, int> BlockStackSizes { get; set; } = new();
    public Dictionary<string, int> ItemStackSizes { get; set; } = new();

    public ConfigStackSizes(ICoreAPI api, ConfigStackSizes previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;
            FillWithDefaultValues = previousConfig.FillWithDefaultValues;

            StackSizeMultiplier = previousConfig.StackSizeMultiplier;
            BlockStackSizes.AddRange(previousConfig.BlockStackSizes);
            ItemStackSizes.AddRange(previousConfig.ItemStackSizes);
        }

        if (api != null && FillWithDefaultValues)
        {
            FillDefault(api);
        }
    }

    public void FillDefault(ICoreAPI api)
    {
        foreach ((string key, int value) in GetDefaultStackSizesForBlocks(api))
        {
            if (!BlockStackSizes.ContainsKey(key))
            {
                BlockStackSizes.Add(key, value);
            }
        }

        foreach ((string key, int value) in GetDefaultStackSizesForItems(api))
        {
            if (!ItemStackSizes.ContainsKey(key))
            {
                ItemStackSizes.Add(key, value);
            }
        }
    }

    public void ApplyPatches(ICoreAPI api)
    {
        if (StackSizeMultiplier is not 0 and not 1)
        {
            foreach (CollectibleObject obj in api.World.Collectibles)
            {
                if (obj.MaxStackSize * obj.MaxStackSize == obj.MaxStackSize)
                {
                    continue;
                }

                obj.MaxStackSize = (int)(obj.MaxStackSize * StackSizeMultiplier);
            }

            return;
        }

        if (BlockStackSizes?.Count != 0)
        {
            foreach ((string key, int value) in BlockStackSizes)
            {
                Block block = api.World.GetBlock(new AssetLocation(key));

                if (block == null || block.Code == null)
                {
                    continue;
                }

                block.MaxStackSize = value;
            }
        }

        if (ItemStackSizes?.Count != 0)
        {
            foreach ((string key, int value) in ItemStackSizes)
            {
                Item item = api.World.GetItem(new AssetLocation(key));

                if (item == null || item.Code == null)
                {
                    continue;
                }

                item.MaxStackSize = value;
            }
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
