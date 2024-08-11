using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

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
        foreach ((string key, int value) in api.GetDefaultStackSizesForBlocks())
        {
            if (!BlockStackSizes.ContainsKey(key))
            {
                BlockStackSizes.Add(key, value);
            }
        }

        foreach ((string key, int value) in api.GetDefaultStackSizesForItems())
        {
            if (!ItemStackSizes.ContainsKey(key))
            {
                ItemStackSizes.Add(key, value);
            }
        }
    }
}
