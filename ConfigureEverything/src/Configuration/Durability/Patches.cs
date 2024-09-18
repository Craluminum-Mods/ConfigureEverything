using Vintagestory.API.Common;

namespace ConfigureEverything.Configuration.ConfigDurability;

public static class Patches
{
    public static void ApplyPatches(this ICoreAPI api, ConfigDurability config)
    {
        if (config.Durability?.Count == 0)
        {
            return;
        }

        foreach ((string key, int value) in config.Durability)
        {
            CollectibleObject obj = api.GetCollectible(key);

            if (obj == null || obj.Code == null)
            {
                continue;
            };

            obj.Durability = value;
        }
    }
}