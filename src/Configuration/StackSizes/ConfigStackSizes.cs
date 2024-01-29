using System.Collections.Generic;
using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration.ConfigStackSizes;

public class ConfigStackSizes : IModConfig
{
    public bool Enabled { get; set; }

    public float StackSizeMultiplier { get; set; } = 1.0f;
    public Dictionary<string, int> BlockStackSizes { get; set; } = new();
    public Dictionary<string, int> ItemStackSizes { get; set; } = new();

    public ConfigStackSizes(ICoreAPI api, ConfigStackSizes previousConfig = null)
    {
        if (previousConfig != null)
        {
            Enabled = previousConfig.Enabled;

            foreach ((string key, int value) in previousConfig.BlockStackSizes)
            {
                if (!BlockStackSizes.ContainsKey(key))
                {
                    BlockStackSizes.Add(key, value);
                }
            }

            foreach ((string key, int value) in previousConfig.ItemStackSizes)
            {
                if (!ItemStackSizes.ContainsKey(key))
                {
                    ItemStackSizes.Add(key, value);
                }
            }

            StackSizeMultiplier = previousConfig.StackSizeMultiplier;
        }

        if (api != null)
        {
            FillDefault(api);
        }
    }

    private void FillDefault(ICoreAPI api)
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
