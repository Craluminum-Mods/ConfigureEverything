using System.Linq;
using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration.ConfigTransitionableProperties;

public static class Patches
{
    public static void ApplyPatches(this ICoreAPI api, ConfigTransitionableProperties config)
    {
        if (config.BlocksTransitionableProperties?.Count != 0)
        {
            foreach ((string key, TransitionableProperties[] value) in config.BlocksTransitionableProperties)
            {
                Block block = api.World.GetBlock(new AssetLocation(key));

                if (block != null && block.Code != null && value.All(x => x.TransitionedStack.Resolve(api.World, "")))
                {
                    block.TransitionableProps = value;
                }
            }
        }

        if (config.ItemsTransitionableProperties?.Count != 0)
        {
            foreach ((string key, TransitionableProperties[] value) in config.ItemsTransitionableProperties)
            {
                Item item = api.World.GetItem(new AssetLocation(key));

                if (item != null && item.Code != null && value.All(x => x.TransitionedStack.Resolve(api.World, "")))
                {
                    item.TransitionableProps = value;
                }
            }
        }
    }
}