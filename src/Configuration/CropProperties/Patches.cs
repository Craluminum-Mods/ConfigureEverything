using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration.ConfigCropProperties;

public static class Patches
{
    public static void ApplyPatches(this ICoreAPI api, ConfigCropProperties config)
    {
        if (config.Crops?.Count == 0)
        {
            return;
        }

        foreach ((string key, BlockCropProperties value) in config.Crops)
        {
            List<Block> blocks = api.World.SearchBlocks(new AssetLocation(key)).ToList();

            if (blocks?.Count == 0)
            {
                continue;
            }

            foreach (Block block in blocks)
            {
                CropBehavior[] behaviors = block.CropProps.Behaviors;
                block.CropProps = value;
                block.CropProps.Behaviors = behaviors;
            }
        }
    }
}